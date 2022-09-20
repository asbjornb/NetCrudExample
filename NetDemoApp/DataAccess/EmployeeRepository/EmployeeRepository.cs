﻿using DataAccess.Model;
using NLog;
using PetaPoco;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataAccess.EmployeeRepository;

//We work with OfficeId's in this class but might as well use e.g. OfficeLocation and put a unique constraint on that
//Maybe even introduce an enum to track locations if those are slow changing.
public class EmployeeRepository
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly string employeeTable = typeof(EmployeePoco).GetCustomAttribute<TableNameAttribute>()!.Value;

    private readonly DatabaseProvider databaseProvider;

    public EmployeeRepository(DatabaseProvider databaseProvider)
    {
        this.databaseProvider = databaseProvider;
    }

    public async Task<int> InsertEmployeeAsync(Employee employee)
    {
        string insertQuery = $"INSERT INTO {employeeTable} ({nameof(EmployeePoco.FirstName)}" +
            $", {nameof(EmployeePoco.LastName)}" +
            $", {nameof(EmployeePoco.Birthdate)}" +
            $", {nameof(EmployeePoco.OfficeId)}) OUTPUT INSERTED.ID " +
            "VALUES (@0, @1, @2, @3)";

        using var database = databaseProvider.GetDatabase();
        try
        {
            return await database.ExecuteScalarAsync<int>(insertQuery, employee.FirstName, employee.LastName, employee.Birthdate, employee.OfficeId);
        }
        catch (Exception)
        {
            Logger.Error("Error inserting employee with database command {0}", database.LastCommand);
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        //In practice it might be better with soft-deletes for a start, but from the spec this seems reasonable
        string deleteQuery = $"DELETE FROM {employeeTable} " +
            $"WHERE {nameof(EmployeePoco.Id)} = @0;";

        using var database = databaseProvider.GetDatabase();
        try
        {
            var result = await database.ExecuteAsync(deleteQuery, id);
            return result > 0;
        }
        catch (Exception)
        {
            Logger.Error("Error deleting employee with database command {0}", database.LastCommand);
            throw;
        }
    }

    //Ideally return Maybe<Employee> but not in scope imo.
    public async Task<Employee?> GetEmployeeAsync(int id)
    {
        string getQuery = $"SELECT {nameof(EmployeePoco.Id)}" +
            $", {nameof(EmployeePoco.FirstName)}" +
            $", {nameof(EmployeePoco.LastName)}" +
            $", {nameof(EmployeePoco.Birthdate)}" +
            $", {nameof(EmployeePoco.OfficeId)}" +
            $" FROM { employeeTable}" +
            $" WHERE {nameof(EmployeePoco.Id)} = @0;";

        using var database = databaseProvider.GetDatabase();
        try
        {
            var result = await database.FetchAsync<EmployeePoco>(getQuery, id);
            var employee = result.SingleOrDefault();
            if(employee is not null)
            {
                return FromPoco(employee);
            }
            return null;
        }
        catch (Exception)
        {
            Logger.Error("Error fetching employee with database command {0}", database.LastCommand);
            throw;
        }
    }

    private static Employee FromPoco(EmployeePoco poco)
    {
        return new Employee(poco.Id, poco.FirstName, poco.LastName, poco.Birthdate, poco.OfficeId);
    }
}
