using System;

namespace DemoApi.Employee.Model;

public record NewEmployee(string FirstName, string LastName, DateTime Birthdate, int OfficeId);
