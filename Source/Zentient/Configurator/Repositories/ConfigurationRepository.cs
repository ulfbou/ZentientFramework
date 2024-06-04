using MongoDB.Driver;
using Zentient.Configurator.Contexts;
using Zentient.Configurator.Schemas;

namespace Zentient.Configurator.Repositories
{
    /// <summary>
    /// Encapsulates database operations related to configurations.
    /// </summary>
    public class ConfigurationRepository
    {
        private readonly ConfigurationContext _context;

        public ConfigurationRepository(ConfigurationContext context)
        {
            _context = context;
        }

        public async Task AddSchema(ConfigurationSchema schema)
        {
            await _context.Schemas.InsertOneAsync(schema);
        }

        public async Task<ConfigurationSchema> GetSchema(string id)
        {
            return await (await _context.Schemas.FindAsync<ConfigurationSchema>(c => c.Id == id)).FirstOrDefaultAsync();
        }

        public async Task AddConfiguration(Configuration config)
        {
            await _context.Configurations.InsertOneAsync(config);
        }

        public async Task<Configuration> GetConfiguration(string id)
        {
            return await (await _context.Configurations.FindAsync<Configurator>(c => c.Id == id)).FirstOrDefaultAsync();
        }

        // Other CRUD operations
    }
}
