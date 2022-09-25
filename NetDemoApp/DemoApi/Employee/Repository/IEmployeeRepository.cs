using DemoApi.Employee.Model;
using System.Threading.Tasks;

namespace DemoApi.Employee.Repository;

public interface IEmployeeRepository
{
    Task<bool> DeleteEmployeeAsync(int id);
    Task<ValidEmployee?> GetEmployeeAsync(int id);
    Task<int> InsertEmployeeAsync(ValidEmployee employee);
    Task<bool> UpdateEmployeeAsync(ValidEmployee employee);
}
