using DataAccess.EmployeeRepository;
using DataAccess.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DemoApi;

internal static class Api
{
    public static void ConfigureApi(this WebApplication app)
    {
        app.MapGet("/Employee/{id}", GetEmployee);
        app.MapPost("/Employees", InsertEmployee);
        app.MapPut("/Employees", UpdateEmployee);
    }

    private static async Task<IResult> GetEmployee(int id, IEmployeeRepository employeeRepository)
    {
        try
        {
            var result = await employeeRepository.GetEmployeeAsync(id);
            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> InsertEmployee(Employee employee, IEmployeeRepository employeeRepository)
    {
        try
        {
            var result = await employeeRepository.InsertEmployeeAsync(employee);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateEmployee(Employee employee, IEmployeeRepository employeeRepository)
    {
        try
        {
            var result = await employeeRepository.UpdateEmployeeAsync(employee);
            if (result)
            {
                return Results.Ok();
            }
            return Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
