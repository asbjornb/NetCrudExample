using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.SqlServer.Dac;
using System.Data.SqlClient;

namespace DemoApi.Tests.DatabaseReliantTests;

[SetUpFixture]
public class SetupDatabaseForTests
{
    private static TestcontainerDatabase testDatabase;
    private const string testDatabaseName = "DemoApiTests";
    private static readonly MsSqlTestcontainerConfiguration configuration = new()
    {
        //For MsSql-testContainers can't set Database or Username.
        //Database defaults to master, Username to sa
        Password = "StrongPa$$word",
    };

    public static string ConnectionString => testDatabase.ConnectionString;
    public static SqlConnection TestDatabaseConnection => new(testDatabase.ConnectionString);

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        //Create container and master database
        testDatabase = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(configuration)
            .Build();
        await testDatabase.StartAsync();
        testDatabase.Database = testDatabaseName; //Connect to new test database rather than master
        //Deploy dacpac
        Deploy("./../../../../Registration/build/Registration.dacpac");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await testDatabase.DisposeAsync();
    }

    private static void Deploy(string dacPacPath)
    {
        var dacOptions = new DacDeployOptions{ CreateNewDatabase = true};
        var dacServiceInstance = new DacServices(testDatabase.ConnectionString);

        using DacPackage dacpac = DacPackage.Load(dacPacPath);
        dacServiceInstance.Deploy(dacpac, testDatabaseName, upgradeExisting: false, options: dacOptions);
    }
}
