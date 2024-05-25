using System;
using System.Collections;

namespace Zentient.Configuration;

public class ConfigurationManager : IEnumerable<KeyValuePair<string, object>>
{
    public ConfigurationManager(Dictionary<string, object> configuration)
    {
        Configuration = configuration;
    }

    public ConfigurationManager(Configurator configurator)
    {
        Configuration = new Dictionary<string, object>(configurator);
    }

    public ConfigurationManager()
    {
        Configuration = new Dictionary<string, object>();
    }

    public Dictionary<string, object> Configuration { get; }

    /// <summary>
    /// Loads configuration settings from various sources(e.g., JSON files, environment variables).
    /// </summary>
    ///   - **LoadConfiguration()**
    ///   - Loads configuration settings from various sources(e.g., JSON files, environment variables).
    ///   - Accepts parameters like configuration file paths, environment names, etc.
    ///   - Returns a configuration object or dictionary.

    public void Load(string path)
    {

    }

    internal void Validate()
    {
        throw new NotImplementedException();
    }

    internal object Get<T>(string v)
    {
        throw new NotImplementedException();
    }

    internal void Set(string v, object debug)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<ConfigurationChangeEventArgs> configurationEvent)
    {
        throw new NotImplementedException();
    }

    public static ConfigurationManager LoadConfiguration(Action<Configurator> configurationAction)
    {
        var configurator = new Configurator();
        configurationAction(configurator);
        return new ConfigurationManager(configurator);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, object>>)Configuration).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Configuration).GetEnumerator();
    }

    private static void AppDomain()
    {
        var configManager = new ConfigurationManager();
        configManager.Load("appsettings.json");
        configManager.Validate();

        var maxRetryAttempts = configManager.Get<int>("MaxRetryAttempts");
        Console.WriteLine($"Max Retry Attempts: {maxRetryAttempts}");

        configManager.Set("LogLevel", LogLevel.Debug);

        configManager.Subscribe(e =>
        {
            Console.WriteLine($"Configuration change detected: {e.Key} = {e.NewValue}");
        });
    }

}
