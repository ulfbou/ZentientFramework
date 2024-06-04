using FluentValidation;
using Zentient.Configurator.Validation;
using FluentValidation.Results;
using MongoDB.Driver;
using Zentient.Configurator.Contexts;
using Zentient.Configurator.Repositories;
using Zentient.Configurator.Schemas;
using Zentient.Configurator;

namespace Zentient.Configurator.Services
{
    /// <summary>
    /// Manages configuration operations such as creation, validation, and retrieval.
    /// </summary>
    public class ConfigurationService
    {
        private readonly ConfigurationContext _context;
        private readonly ConfigurationRepository _repository;
        public ConfigurationService(ConfigurationContext context)
        {
            _context = context;
            _repository = new ConfigurationRepository(context);
        }

        public async Task CreateSchema(Action<ConfigurationSchemaBuilder> builderAction)
        {
            var builder = new ConfigurationSchemaBuilder();
            builderAction(builder);
            var schema = builder.Build();

            // Store schema in the database or a schema repository
            try
            {
                await _repository.AddSchema(schema);
            }
            catch (ValidationException ex)
            {
                throw new ValidationException($"Schema `{schema.Name}` is not valid.", ex.Errors);
            }
        }

        public async Task CreateConfiguration(Action<ConfigurationBuilder> builderAction)
        {
            var builder = new ConfigurationBuilder();
            builderAction(builder);

            var configuration = builder.Build();
            var schema = await GetSchema(configuration.SchemaName);

            if (schema.TryValidate(configuration, out ValidationResult validationResult))
            {
                await _context.Configurations.InsertOneAsync(configuration);
            }
            else
            {
                throw new ValidationException($"Configuration `{configuration.SchemaName}`is not valid.", validationResult?.Errors);
            }
        }

        public async Task<ConfigurationSchema> GetSchema(string schemaName)
        {
            return await _repository.GetSchema(schemaName);
        }

        public async Task ApplyConfigurationTo(IConfigurable configurable, string configId)
        {
            var configuration = await (await _context.Configurations.FindAsync(c => c.Id == configId)).FirstOrDefaultAsync();

            if (configuration != null)
            {
                configurable.ApplyConfiguration(configuration);
            }
            else
            {
                throw new Exception("Configuration not found.");
            }
        }
    }
}
