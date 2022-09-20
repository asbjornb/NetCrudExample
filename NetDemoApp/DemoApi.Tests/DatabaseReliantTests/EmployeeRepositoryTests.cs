#nullable disable

using DataAccess;
using DataAccess.EmployeeRepository;
using DataAccess.Model;
using FluentAssertions;
using NUnit.Framework;
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
    public async Task ShouldInsertNewEmployeeAsync()
    {
        var employee = new Employee(null, FirstName: "John", LastName: "Doe", Birthdate: new DateTime(1990, 1, 1), OfficeId: 1);
        var result = await sut.InsertEmployee(employee);

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
}
