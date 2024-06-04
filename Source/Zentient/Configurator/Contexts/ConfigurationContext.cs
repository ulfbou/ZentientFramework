
using MongoDB.Driver;
using Zentient.Configurator.Schemas;

namespace Zentient.Configurator.Contexts
{
    /// <summary>
    /// Acts as a bridge between the application and the database. Manages the MongoDB connection and provides access to the configuration data.
    /// </summary>
    public class ConfigurationContext
    {
        private readonly IMongoDatabase _database;

        public ConfigurationContext(string connectionString)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("ConfigurationDb");
        }

        public IMongoCollection<Configuration> Configurations => _database.GetCollection<Configurator>("Configurations");
        public IMongoCollection<ConfigurationSchema> Schemas => _database.GetCollection<ConfigurationSchema>("Schemas");
    }
}
