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
    private EmployeeRepository sut;
    private DatabaseProvider databaseProvider;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        //Create repository with test-database-connection and default MS Sql provider
        databaseProvider = new DatabaseProvider(SetupDatabaseForTests.ConnectionString);

        //Insert office for foreign key
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync("INSERT INTO reg.Offices(Location, MaxOccupancy) VALUES(@0, @1);", "TestOffice", 10);
    }

    [SetUp]
    public void SetUp()
    {
        sut = new EmployeeRepository(databaseProvider);
    }

    [TearDown]
    public async Task TearDown()
    {
        //Reset table after each test
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"TRUNCATE TABLE {employeeTable};");
    }

    [Test]
    public async Task ShouldInsertNewEmployee()
    {
        var employee = new Employee(null, FirstName: "John", LastName: "Doe", Birthdate: new DateTime(1990, 1, 1), OfficeId: 1);
        var result = await sut.InsertEmployeeAsync(employee);

        using var database = databaseProvider.GetDatabase();
        var rows = database.Fetch<EmployeePoco>();
        rows.Should().HaveCount(1);
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
        using var database = databaseProvider.GetDatabase();
        database.Execute("SET IDENTITY_INSERT reg.Employees ON; " +
            "INSERT INTO reg.Employees(Id, FirstName, LastName, Birthdate, OfficeId) VALUES(@0, @1, @2, @3, @4); " +
            "SET IDENTITY_INSERT reg.Employees OFF;", id, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.DeleteEmployeeAsync(id);

        var rows = database.Fetch<EmployeePoco>();
        rows.Should().HaveCount(0);
        result.Should().Be(true);
    }

    [Test]
    public async Task ShouldNotDeleteOtherEmployees()
    {
        using var database = databaseProvider.GetDatabase();
        database.Execute("SET IDENTITY_INSERT reg.Employees ON; " +
            "INSERT INTO reg.Employees(Id, FirstName, LastName, Birthdate, OfficeId) VALUES(@0, @1, @2, @3, @4); " +
            "SET IDENTITY_INSERT reg.Employees OFF;", 1, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.DeleteEmployeeAsync(5);

        var rows = database.Fetch<EmployeePoco>();
        rows.Should().HaveCount(1);
        result.Should().Be(false);
    }

    [Test]
    public async Task ShouldGetEmployeeIfExists()
    {
        const int id = 1;
        const string firstName = "John";
        const string lastName = "Doe";
        var birthDate = new DateTime(1990, 1, 1);
        const int officeId = 1;
        
        using var database = databaseProvider.GetDatabase();
        database.Execute("SET IDENTITY_INSERT reg.Employees ON; " +
            "INSERT INTO reg.Employees(Id, FirstName, LastName, Birthdate, OfficeId) VALUES(@0, @1, @2, @3, @4); " +
            "SET IDENTITY_INSERT reg.Employees OFF;", id, firstName, lastName, birthDate, officeId);

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
        using var database = databaseProvider.GetDatabase();
        database.Execute("SET IDENTITY_INSERT reg.Employees ON; " +
            "INSERT INTO reg.Employees(Id, FirstName, LastName, Birthdate, OfficeId) VALUES(@0, @1, @2, @3, @4); " +
            "SET IDENTITY_INSERT reg.Employees OFF;", 1, "John", "Doe", new DateTime(1990, 1, 1), 1);

        var result = await sut.GetEmployeeAsync(5);

        result.Should().BeNull();
    }
}
