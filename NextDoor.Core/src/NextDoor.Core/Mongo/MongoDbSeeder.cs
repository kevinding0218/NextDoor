using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NextDoor.Core.Types;

namespace NextDoor.Core.Mongo
{
    public class MongoDbSeeder : IDataSeeder
    {

        public readonly IMongoDatabase Database;
        public MongoDbSeeder(IMongoDatabase database)
        {
            Database = database;
        }

        public async Task SeedAsync()
        {
            await this.CustomSeedAsync();
        }

        protected virtual async Task CustomSeedAsync()
        {
            var cursor = await Database.ListCollectionsAsync();
            var collections = await cursor.ToListAsync();
            if (collections.Any())
            {
                return;
            }
            await Task.CompletedTask;
        }
    }
}