using System.Collections.Generic;

namespace DemoApi.Employee.Model;

public sealed class ValidationResult
{
    public readonly List<string> Errors = new();
    public ValidEmployee? ValidEmployee { get; }

    private ValidationResult(EmployeeModel? employee, List<string>? errors)
    {
        if (employee != null)
        {
            ValidEmployee = new ValidEmployee(employee);
        }
        if (errors is not null && errors.Count > 0)
        {
            Errors.AddRange(errors);
        }
    }

    public bool IsValid => Errors.Count == 0;

    internal static ValidationResult Succes(EmployeeModel employee)
    {
        return new ValidationResult(employee, null);
    }

    internal static ValidationResult Fail(List<string> errors)
    {
        return new ValidationResult(null, errors);
    }
}
