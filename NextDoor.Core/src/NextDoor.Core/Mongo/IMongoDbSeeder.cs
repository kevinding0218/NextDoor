using System.Threading.Tasks;

namespace NextDoor.Core.Mongo
{
    public interface IMongoDbSeeder
    {
         Task SeedAsync();
    }
}