#nullable disable

using DataAccess;
using DemoApi.Employee.Model;
using DemoApi.Employee.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using PetaPoco;
using System.Net;
using System.Reflection;
using System.Text;

namespace DemoApi.Tests.DatabaseReliantTests;

public class ApiTests
{
    private readonly string employeeTable = typeof(EmployeePoco).GetCustomAttribute<TableNameAttribute>().Value;
    private const string officeTable = "reg.Offices";
    private readonly EmployeeModel testEmployee = new(1, "John", "Doe", new DateTime(2000, 1, 1), 1);

    private DatabaseProvider databaseProvider;
    private WebApplicationFactory<Program> appFactory;
    private HttpClient client;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        //Create repository with test-database-connection and default MS Sql provider
        databaseProvider = new DatabaseProvider(SetupDatabaseForTests.ConnectionString);

        //Insert office for foreign key
        await InsertOffice(1, "TestOffice", 2);
        //Insert single employee for tests
        await InsertEmployeeWithId(testEmployee.Id.Value, testEmployee.FirstName, testEmployee.LastName
            , testEmployee.Birthdate, testEmployee.OfficeId);

        //Set up a test-HttpClient
        appFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(x =>
            {
                //Use test-database rather than config
                x.RemoveAll(typeof(IDatabaseProvider));
                x.AddSingleton<IDatabaseProvider>(_ => databaseProvider);
            }));
        client = appFactory.CreateClient();
    }

    [TearDown]
    public async Task TearDown()
    {
        //Reset table after each test
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"DELETE FROM {employeeTable} WHERE Id>@0;", 1);
        await database.ExecuteAsync($"DELETE FROM {officeTable} WHERE Id>@0;", 1);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        //Reset table after all tests
        using var database = databaseProvider.GetDatabase();
        await database.ExecuteAsync($"TRUNCATE TABLE {employeeTable};");
        await database.ExecuteAsync($"DELETE FROM {officeTable};");

        //Dispose HttpClient and factory
        client.Dispose();
        await appFactory.DisposeAsync();
    }

    [Test]
    public async Task GetShouldReturnUser()
    {
        var response = await client.GetAsync("/Employee/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<EmployeeModel>(content);
        result.Id.Should().Be(testEmployee.Id);
        result.FirstName.Should().Be(testEmployee.FirstName);
        result.LastName.Should().Be(testEmployee.LastName);
        result.Birthdate.Should().Be(testEmployee.Birthdate);
        result.OfficeId.Should().Be(testEmployee.OfficeId);
    }

    [Test]
    public async Task GetShouldReturnNotFoundIfNotExisting()
    {
        var response = await client.GetAsync("/Employee/2");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task PostShouldInsertUser()
    {
        var employee = new NewEmployee("Jane", "Doe", new DateTime(2000, 1, 1), 1);
        var postContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/Employees", postContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<int>(content);

        using var database = databaseProvider.GetDatabase();
        var employees = await database.FetchAsync<EmployeePoco>("WHERE Id=@0;", result);
        employees.Should().ContainSingle();
        var employeePoco = employees.Single();
        employeePoco.FirstName.Should().Be(employee.FirstName);
        employeePoco.LastName.Should().Be(employee.LastName);
        employeePoco.Birthdate.Should().Be(employee.Birthdate);
        employeePoco.OfficeId.Should().Be(employee.OfficeId);
    }

    [Test]
    public async Task PutShouldUpdateUser()
    {
        //Insert a dummy to update
        const int id = 2;
        await InsertOffice(2, "TestOffice2", 10);
        await InsertEmployeeWithId(id, testEmployee.FirstName, testEmployee.LastName
            , testEmployee.Birthdate, testEmployee.OfficeId);
        var updatedEmployee = new EmployeeModel(id, "Jane", "Doe", new DateTime(1980, 2, 3), 2);

        var postContent = new StringContent(JsonConvert.SerializeObject(updatedEmployee), Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/Employees", postContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var database = databaseProvider.GetDatabase();
        var employees = await database.FetchAsync<EmployeePoco>("WHERE Id=@0;", id);
        employees.Should().ContainSingle();
        var employeePoco = employees.Single();
        employeePoco.FirstName.Should().Be(updatedEmployee.FirstName);
        employeePoco.LastName.Should().Be(updatedEmployee.LastName);
        employeePoco.Birthdate.Should().Be(updatedEmployee.Birthdate);
        employeePoco.OfficeId.Should().Be(updatedEmployee.OfficeId);
    }

    [Test]
    public async Task PutShouldReturnNotFoundIfNotExisting()
    {
        var updatedEmployee = new EmployeeModel(5, "Jane", "Doe", new DateTime(1980, 2, 3), 1);

        var postContent = new StringContent(JsonConvert.SerializeObject(updatedEmployee), Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/Employees", postContent);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteShouldDeleteUser()
    {
        //Insert a dummy to update
        const int id = 2;
        await InsertEmployeeWithId(id, testEmployee.FirstName, testEmployee.LastName
            , testEmployee.Birthdate, testEmployee.OfficeId);

        var response = await client.DeleteAsync($"/Employees/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var database = databaseProvider.GetDatabase();
        var employees = await database.FetchAsync<EmployeePoco>("WHERE Id=@0;", id);
        employees.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteShouldReturnNotFoundIfNotExisting()
    {
        var response = await client.DeleteAsync("/Employees/5");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
