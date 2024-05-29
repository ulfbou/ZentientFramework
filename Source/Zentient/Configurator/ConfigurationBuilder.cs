using Zentient.Configurator.Schemas;

namespace Zentient.Configurator
{
    /// <summary>
    /// Provides a fluent interface for constructing Configuration instances.
    /// </summary>
    public class ConfigurationBuilder
    {
        private readonly Configuration _configuration;

        public ConfigurationBuilder()
        {
            _configuration = new Configuration { Data = new Dictionary<string, object>() };
        }

        public ConfigurationBuilder WithSchema(string schemaName)
        {
            ArgumentNullException.ThrowIfNull(schemaName, nameof(schemaName));
            if (string.IsNullOrEmpty(schemaName))
            {
                throw new ArgumentException("SchemaName cannot be null or empty.", nameof(schemaName));
            }

            _configuration.SchemaName = schemaName;
            return this;
        }

        public ConfigurationBuilder WithProperty(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            _configuration.SetProperty(key, value);
            return this;
        }

        public Configuration Build() => _configuration;
    }
}
