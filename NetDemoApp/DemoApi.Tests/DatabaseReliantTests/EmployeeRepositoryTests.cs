#nullable disable

using DataAccess;
using DataAccess.EmployeeRepository;
using DataAccess.Model;
using FluentAssertions;
using PetaPoco;
using System.Reflection;

namespace DemoApi.Tests.DatabaseReliantTests;

public class EmployeeRepositoryTests
{
    private readonly string employeeTable = typeof(EmployeePoco).GetCustomAttribute<TableNameAttribute>().Value;
    private const string officeTable = "reg.Offices";
    private EmployeeSqlRepository sut;
    private DatabaseProvider databaseProvider;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        //Create repository with test-database-connection and default MS Sql provider
        databaseProvider = new DatabaseProvider(SetupDatabaseForTests.ConnectionString);

        //Insert office for foreign key
        await InsertOffice(1, "TestOffice", 2);
    }

    [SetUp]
    public void SetUp()
    {
        sut = new EmployeeSqlRepository(databaseProvider);
    }

    [TearDown]
    public async Task TearDown()
    {
        //Reset table after each test
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"TRUNCATE TABLE {employeeTable};");
        await database.ExecuteAsync($"DELETE FROM {officeTable} WHERE Id>@0;", 1);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        //Reset table after each test
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"TRUNCATE TABLE {employeeTable};");
        await database.ExecuteAsync($"DELETE FROM {officeTable};");
    }

    [Test]
    public async Task ShouldInsertNewEmployee()
    {
        var employee = new Employee(null, FirstName: "John", LastName: "Doe", Birthdate: new DateTime(1990, 1, 1), OfficeId: 1);
        var result = await sut.InsertEmployeeAsync(employee);

        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().ContainSingle();
        var row = rows.Single();
        row.FirstName.Should().Be(employee.FirstName);
        row.LastName.Should().Be(employee.LastName);
        row.Birthdate.Should().Be(employee.Birthdate);
        row.OfficeId.Should().Be(1);
        result.Should().Be(row.Id);
    }

    [Test]
    public async Task ShouldDeleteEmployee()
    {
        const int id = 1;

        await InsertEmployeeWithId(id, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.DeleteEmployeeAsync(id);

        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().HaveCount(0);
        result.Should().BeTrue();
    }

    [Test]
    public async Task ShouldNotDeleteOtherEmployees()
    {
        await InsertEmployeeWithId(1, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.DeleteEmployeeAsync(5);

        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().ContainSingle();
        result.Should().BeFalse();
    }

    [Test]
    public async Task ShouldGetEmployeeIfExists()
    {
        const int id = 1;
        const string firstName = "John";
        const string lastName = "Doe";
        var birthDate = new DateTime(1990, 1, 1);
        const int officeId = 1;

        await InsertEmployeeWithId(id, firstName, lastName, birthDate, officeId);

        var result = await sut.GetEmployeeAsync(id);

        result.Id.Should().Be(id);
        result.FirstName.Should().Be(firstName);
        result.LastName.Should().Be(lastName);
        result.Birthdate.Should().Be(birthDate);
        result.OfficeId.Should().Be(officeId);
    }

    [Test]
    public async Task ShouldGetNullIfEmployeeNotExists()
    {
        await InsertEmployeeWithId(1, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.GetEmployeeAsync(5);

        result.Should().BeNull();
    }

    [Test]
    public async Task ShouldUpdateEmployeeIfExists()
    {
        var employee = new Employee(1, FirstName: "John", LastName: "Doe", Birthdate: new DateTime(1990, 1, 1), OfficeId: 1);

        await InsertEmployeeWithId(employee.Id.Value, employee.FirstName, employee.LastName, employee.Birthdate, employee.OfficeId);
        await InsertOffice(2, "AnotherOffice", 10);

        var updatedEmployee = employee with { FirstName = "Jane", LastName = "Ford", Birthdate = new DateTime(1991, 2, 2), OfficeId = 2 };

        var result = await sut.UpdateEmployeeAsync(updatedEmployee);

        result.Should().BeTrue();
        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().ContainSingle();
        var row = rows.Single();
        row.FirstName.Should().Be(updatedEmployee.FirstName);
        row.LastName.Should().Be(updatedEmployee.LastName);
        row.Birthdate.Should().Be(updatedEmployee.Birthdate);
        row.OfficeId.Should().Be(2);
    }

    [Test]
    public async Task UpdateShouldReturnFalseIfEmployeeNotExists()
    {
        var employee = new Employee(1, "John", "Doe", new DateTime(1990, 1, 1), 1);

        await InsertEmployeeWithId(1, employee.FirstName, employee.LastName, employee.Birthdate, employee.OfficeId);

        var updatedEmployee = employee with { Id = 5, FirstName = "Jane" };

        var result = await sut.UpdateEmployeeAsync(updatedEmployee);

        result.Should().BeFalse();
        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().ContainSingle();
        var row = rows.Single();
        row.FirstName.Should().Be(employee.FirstName);
    }

    private async Task InsertEmployeeWithId(int id, string firstName, string lastName, DateTime birthDate, int officeId)
    {
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"SET IDENTITY_INSERT {employeeTable} ON; " +
                    $"INSERT INTO {employeeTable}(Id, FirstName, LastName, Birthdate, OfficeId) VALUES(@0, @1, @2, @3, @4); " +
                    $"SET IDENTITY_INSERT {employeeTable} OFF;", id, firstName, lastName, birthDate, officeId);
    }

    private async Task InsertOffice(int id, string location, int maxOccupancy)
    {
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"SET IDENTITY_INSERT {officeTable} ON; INSERT INTO {officeTable}(Id, Location, MaxOccupancy) VALUES(@0, @1, @2); SET IDENTITY_INSERT {officeTable} OFF; ", id, location, maxOccupancy);
    }
}
