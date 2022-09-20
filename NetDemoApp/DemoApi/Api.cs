using DataAccess.EmployeeRepository;
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
}
