using System;

namespace DataAccess.Model;

public record Employee(int? Id, string FirstName, string LastName, DateTime Birthdate, int OfficeId);
