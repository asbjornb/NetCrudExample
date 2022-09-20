#nullable disable
using PetaPoco;
using System;

namespace DataAccess.EmployeeRepository;

[TableName("reg.Employees")]
[PrimaryKey("Id", AutoIncrement = true)]
internal class EmployeePoco
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    public int OfficeId { get; set; }
}
