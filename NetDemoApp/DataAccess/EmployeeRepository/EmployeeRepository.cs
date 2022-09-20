using DataAccess.Model;
using NLog;
using PetaPoco;
using System.Reflection;
using System.Threading.Tasks;

namespace DataAccess.EmployeeRepository;

public class EmployeeRepository
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly string employeeTable = typeof(EmployeePoco).GetCustomAttribute<TableNameAttribute>()!.Value;

    private readonly DatabaseProvider databaseProvider;
    public EmployeeRepository(DatabaseProvider databaseProvider)
    {
        this.databaseProvider = databaseProvider;
    }

    public async Task<int> InsertEmployee(Employee employee)
    {
        string insertQuery = $"INSERT INTO {employeeTable} ({nameof(EmployeePoco.FirstName)}" +
            $", {nameof(EmployeePoco.LastName)}" +
            $", {nameof(EmployeePoco.Birthdate)}" +
            $", {nameof(EmployeePoco.OfficeId)}) OUTPUT INSERTED.ID " +
            "VALUES (@0, @1, @2, @3)";

        using var database = databaseProvider.GetDatabase();
        try
        {
            return await database.ExecuteAsync(insertQuery, employee.FirstName, employee.LastName, employee.Birthdate, employee.OfficeId);
        }
        catch (System.Exception)
        {
            Logger.Error("Error inserting employee with database command {0}", database.LastCommand);
            throw;
        }
    }
}
