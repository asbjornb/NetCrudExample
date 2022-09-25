using System;

namespace DemoApi.Employee.Model;

public record ValidEmployee
{
    internal ValidEmployee(int? id, string firstName, string lastName, DateTime birthdate, int officeId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Birthdate = birthdate;
        OfficeId = officeId;
    }

    internal ValidEmployee(EmployeeModel emp) : this(emp.Id, emp.FirstName, emp.LastName, emp.Birthdate, emp.OfficeId) { }

    public int? Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime Birthdate { get; init; }
    public int OfficeId { get; init; }
}