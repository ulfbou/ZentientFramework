namespace Zentient.Configurator.Schemas
{
    /// <summary>
    /// Provides a fluent interface for constructing ConfigurationSchema instances.
    /// </summary>
    public class ConfigurationSchemaBuilder
    {
        private readonly ConfigurationSchema _schema
            = new ConfigurationSchema { Properties = new Dictionary<string, PropertyDefinition>() };

        public ConfigurationSchemaBuilder WithName(string name)
        {
            _schema.Name = name;
            return this;
        }

        public ConfigurationSchemaBuilder WithProperty(string key, string type, bool isRequired = true)
        {
            _schema.Properties[key] = new PropertyDefinition
            {
                Type = type,
                IsRequired = isRequired
            };

            return this;
        }

        public ConfigurationSchema Build() => _schema;
    }
}