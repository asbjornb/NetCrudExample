using DataAccess.Model;

namespace DataAccess
{
    public interface IEmployeeValidator
    {
        ValidationResult Validate(Employee employee);
    }
}