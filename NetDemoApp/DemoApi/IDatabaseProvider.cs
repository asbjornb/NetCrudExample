using PetaPoco;

namespace DataAccess
{
    public interface IDatabaseProvider
    {
        IDatabase GetDatabase();
    }
}