using FluentValidation;
using Zentient.Configurator.Validation;
using MongoDB.Driver;
using Zentient.Configurator.Contexts;
using Zentient.Configurator.Repositories;
using Zentient.Configurator.Schemas;
using Newtonsoft.Json;

namespace Zentient.Configurator.Services
{
    /// <summary>
    /// Manages configuration operations such as creation, validation, and retrieval.
    /// </summary>
    public class ConfigurationService
    {
        private const string _configurationPath = "configurations";

        private readonly ConfigurationContext _context;
        private readonly ConfigurationRepository _repository;
        private readonly ConfigurationValidator _validator;

        public ConfigurationService(ConfigurationContext context, ConfigurationValidator validator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
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
                await _repository.AddSchemaAsync(schema);
            }
            catch (ValidationException ex)
            {
                throw new ValidationException($"Schema `{schema.Name}` is not valid.", ex.Errors);
            }
        }

        public async Task CreateConfiguration(Action<ConfigurationBuilder> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction, nameof(builderAction));

            var builder = new ConfigurationBuilder();
            builderAction(builder);

            var configuration = builder.Build();
            var schema = await GetSchema(configuration.SchemaName);

            if (schema.TryValidate(configuration, out var validationResult))
            {
                await _context.Configurations.InsertOneAsync(configuration);
            }
            else
            {
                throw new ValidationException($"Configuration `{configuration.SchemaName}` is not valid.", validationResult.Errors);
            }
        }

        public async Task<ConfigurationSchema> GetSchema(string schemaName)
        {
            return await _repository.GetSchemaAsync(schemaName);
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
                throw new Exception($"Configuration for `{configId}` is not found.");
            }
        }

        public async Task<Configuration> LoadConfigurationAsync(string configId)
        {
            var data = await LoadDataAsync(configId);
            var configuration = new Configuration { Id = configId };

            foreach (var pair in data)
            {
                configuration.SetProperty(pair.Key, pair.Value);
            }

            return configuration;
        }

        private async Task<Dictionary<string, object>> LoadDataAsync(string configId)
        {
            var data = new Dictionary<string, object>();
            var filePath = Path.Combine(_configurationPath, $"{configId}.json");

            try
            {
                if (!Directory.Exists(_configurationPath))
                {
                    Directory.CreateDirectory(_configurationPath);
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Configuration file `{filePath}` not found.");
                }

                var json = await File.ReadAllTextAsync(filePath);
                data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? throw new JsonException($"Failed to deserialize configuration data for `{configId}`.");
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to read configuration file `{filePath}`.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access to configuration file `{filePath}` is denied.", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize configuration data for `{configId}`.", ex);
            }

            return data;
        }
    }
}
