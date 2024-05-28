namespace Zentient.Configurator.Schemas
{
    /// <summary>
    /// Provides a fluent interface for constructing ConfigurationSchema instances.
    /// </summary>
    public class ConfigurationSchemaBuilder
    {
        private readonly ConfigurationSchema _schema;

        public ConfigurationSchemaBuilder()
        {
            _schema = new ConfigurationSchema { Properties = new Dictionary<string, string>() };
        }

        public ConfigurationSchemaBuilder WithName(string name)
        {
            _schema.Name = name;
            return this;
        }

        public ConfigurationSchemaBuilder WithProperty(string key, string type)
        {
            _schema.Properties[key] = type;
            return this;
        }

        public ConfigurationSchema Build() => _schema;
    }
}