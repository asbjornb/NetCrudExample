using DemoApi.Employee.Model;
using DemoApi.Employee.Repository;
using DemoApi.Employee.Validation;
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
        app.MapDelete("/Employees/{id}", DeleteEmployee);
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

    private static async Task<IResult> InsertEmployee(NewEmployee employee, IEmployeeValidator employeeValidator, IEmployeeRepository employeeRepository)
    {
        try
        {
            var validationResult = employeeValidator.Validate(employee);
            if (validationResult.IsValid)
            {
                var result = await employeeRepository.InsertEmployeeAsync(validationResult.ValidEmployee!);
                return Results.Ok(result);
            }
            return Results.BadRequest(validationResult.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateEmployee(EmployeeModel employee, IEmployeeValidator employeeValidator, IEmployeeRepository employeeRepository)
    {
        try
        {
            var validationResult = employeeValidator.Validate(employee);
            if (validationResult.IsValid)
            {
                var result = await employeeRepository.UpdateEmployeeAsync(validationResult.ValidEmployee!);
                if (result)
                {
                    return Results.Ok();
                }
                return Results.NotFound();
            }
            return Results.BadRequest(validationResult.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteEmployee(int id, IEmployeeRepository employeeRepository)
    {
        try
        {
            var result = await employeeRepository.DeleteEmployeeAsync(id);
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
