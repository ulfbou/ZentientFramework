namespace Zentient.Configurator
{
    /// <summary>
    /// Represents an instance of configuration data.
    /// </summary>
    public class Configuration
    {
        public string Id { get; set; }
        public string SchemaName { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
