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
            _configuration = new Configuration { Data = new Dictionary<string, string>() };
        }

        public ConfigurationBuilder WithSchema(string schemaName)
        {
            _configuration.SchemaName = schemaName;
            return this;
        }

        public ConfigurationBuilder WithProperty(string key, string value)
        {
            _configuration.Data[key] = value;
            return this;
        }

        public Configuration Build() => _configuration;
    }
}
