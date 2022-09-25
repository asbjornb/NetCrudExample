using System;

namespace DemoApi.Employee.Model;

public record EmployeeModel(int? Id, string FirstName, string LastName, DateTime Birthdate, int OfficeId) : NewEmployee(FirstName, LastName, Birthdate, OfficeId);
