using Zentient.Configurator.Schemas;

namespace Zentient.Configurator
{
    /// <summary>
    /// Represents an instance of configuration data.
    /// </summary>
    public class Configuration
    {
        public string Id { get; set; }
        public string SchemaName { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public void SetProperty(string key, object value)
        {
            if (Data is null)
            {
                Data = new Dictionary<string, object>();
            }

            Data[key] = value;
        }
    }
}
