namespace Zentient.Configurator.Schemas
{
    /// <summary>
    /// Represents a schema that can be applied to configurations, defining their structure and constraints.
    /// </summary>
    public abstract class Schema
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, PropertyDefinition> Properties { get; set; } = new Dictionary<string, PropertyDefinition>();
    }
}
