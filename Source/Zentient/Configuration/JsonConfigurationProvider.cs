using Newtonsoft.Json.Linq;

namespace Zentient.Configuration;

public class JsonConfigurationProvider : IConfigurationProvider
{
    private readonly string _filePath;

    public JsonConfigurationProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ConfigurationBuilder Load()
    {
        var builder = new ConfigurationBuilder();

        if (!File.Exists(_filePath)) throw new FileNotFoundException($"Configuration file not found at '{_filePath}'.");

        var jsonString = File.ReadAllText(_filePath);
        var jsonObject = JObject.Parse(jsonString);

        foreach (var property in jsonObject.Properties())
        {
            if (property.Value is null) throw new InvalidOperationException($"Property value `{property.Name}` is null.");
            builder.WithSetting(property.Name, property.Value.ToObject<object>()!);
        }

        return builder;
    }

    public void Load(ConfigurationBuilder builder)
    {
        if (!File.Exists(_filePath)) throw new FileNotFoundException($"Configuration file not found at '{_filePath}'.");

        var jsonString = File.ReadAllText(_filePath);
        var jsonObject = JObject.Parse(jsonString);

        foreach (var property in jsonObject.Properties())
        {
            builder.WithSetting(property.Name, property.Value.ToObject<object>());
        }
    }
}

