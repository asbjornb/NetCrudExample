using DemoApi.Employee.Model;

namespace DemoApi.Employee.Validation;

public interface IEmployeeValidator
{
    ValidationResult Validate(EmployeeModel employee);
    ValidationResult Validate(NewEmployee employee);
}
