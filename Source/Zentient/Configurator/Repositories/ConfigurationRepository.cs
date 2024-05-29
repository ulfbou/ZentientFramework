using MongoDB.Bson;
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
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(context.Configurations);
            _context = context;
        }

        public async Task AddSchemaAsync(ConfigurationSchema schema)
        {
            ArgumentNullException.ThrowIfNull(schema);
            await _context.Schemas.InsertOneAsync(schema);
        }

        public async Task AddConfigurationAsync(Configuration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            await _context.Configurations.InsertOneAsync(configuration);
        }

        public async Task<ConfigurationSchema> GetSchemaAsync(string id)
        {
            return await (await _context.Schemas.FindAsync<ConfigurationSchema>(c => c.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<Configuration> GetConfigurationAsync(string id)
        {
            return await (await _context.Configurations.FindAsync<Configuration>(c => c.Id == id)).FirstOrDefaultAsync();
        }
        public async Task BackupDataAsync(string backupFilePath)
            => await _context.BackupDataAsync(backupFilePath);

        public async Task RestoreData(string backupFilePath)
            => await _context.RestoreData(backupFilePath);
    }
}
