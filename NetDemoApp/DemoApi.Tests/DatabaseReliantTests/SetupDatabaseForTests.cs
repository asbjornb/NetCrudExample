using Microsoft.SqlServer.Dac;
using System.Data.SqlClient;

namespace DemoApi.Tests.DatabaseReliantTests;

[SetUpFixture]
public class SetupDatabaseForTests
{
    private const string testDatabaseName = "DemoApiTests";
    private const string dacPacPath = "./../../../../Registration/build/Registration.dacpac";

    public static string ConnectionString => $"Server=localhost;Database={testDatabaseName};Integrated Security=True;ApplicationIntent=ReadWrite";
    public static SqlConnection TestDatabaseConnection => new(ConnectionString);

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        //Deploy dacpac
        var dacOptions = new DacDeployOptions { CreateNewDatabase = true };
        var dacServiceInstance = new DacServices(ConnectionString);

        using DacPackage dacpac = DacPackage.Load(dacPacPath);
        dacServiceInstance.Deploy(dacpac, testDatabaseName, upgradeExisting: true, options: dacOptions);
    }

    //Could consider dropping database on OneTimeTearDown, but it can be nice enough to check state
    //Could use sql in docker container for tests to simplify build pipelines but tests are slower
}
