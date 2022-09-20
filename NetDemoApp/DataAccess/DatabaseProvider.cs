using PetaPoco;

namespace DataAccess;

public class DatabaseProvider
{
    private readonly string connectionString;
    private readonly string providerName;

    public DatabaseProvider(string connectionString, string providerName="System.Data.SqlClient")
    {
        this.connectionString = connectionString;
        this.providerName = providerName;
    }

    public IDatabase GetDatabase()
    {
        return new Database(connectionString, providerName);
    }
}
