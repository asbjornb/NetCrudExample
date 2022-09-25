using DemoApi.Employee.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoApi.Employee.Validation;

public class EmployeeValidator : IEmployeeValidator
{
    private const int MinAge = 14;
    private const int MaxAge = 120;

    //Validate employee and return ValidationResult
    public ValidationResult Validate(EmployeeModel employee)
    {
        var errors = new List<string>();
        //Validate that firstname is not empty
        if (string.IsNullOrWhiteSpace(employee.FirstName))
        {
            errors.Add("Firstname is required");
        }
        //Validate that lastname is not empty
        if (string.IsNullOrWhiteSpace(employee.LastName))
        {
            errors.Add("Lastname is required");
        }
        //Validate that lastname has no whitespace
        if (employee.LastName.Any(char.IsWhiteSpace))
        {
            errors.Add("Lastname cannot contain whitespace");
        }
        //Validate birthdate is "reasonable"
        if (GetAge(employee.Birthdate) < MinAge || GetAge(employee.Birthdate) > MaxAge)
        {
            errors.Add($"Error in birthdate. Age is outside range {MinAge}-{MaxAge}");
        }
        //return
        if (errors.Count > 0)
        {
            return ValidationResult.Fail(errors);
        }
        return ValidationResult.Succes(employee);
    }

    public ValidationResult Validate(NewEmployee employee)
    {
        return Validate(new EmployeeModel(null, employee.FirstName, employee.LastName, employee.Birthdate, employee.OfficeId));
    }

    private static int GetAge(DateTime birthdate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthdate.Year;
        if (birthdate > today.AddYears(-age))
        {
            age--;
        }
        return age;
    }
}
