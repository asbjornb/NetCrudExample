namespace DataAccess.EmployeeRepository;

public class EmployeeRepository
{
    private readonly DatabaseProvider databaseProvider;
    public EmployeeRepository(DatabaseProvider databaseProvider)
    {
        this.databaseProvider = databaseProvider;
    }
}
