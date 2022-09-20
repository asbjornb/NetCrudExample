using DataAccess.Model;
using System.Threading.Tasks;

namespace DataAccess.EmployeeRepository;

public interface IEmployeeRepository
{
    Task<bool> DeleteEmployeeAsync(int id);
    Task<Employee?> GetEmployeeAsync(int id);
    Task<int> InsertEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
}
