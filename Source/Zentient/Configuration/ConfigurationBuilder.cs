using System.Collections;

namespace Zentient.Configuration;

public class ConfigurationBuilder : IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _configuration = new Dictionary<string, object>();

    public ConfigurationBuilder AddEnvironmentVariables()
    {
        foreach (string key in Environment.GetEnvironmentVariables().Keys)
        {
            if (_configuration.ContainsKey(key)) throw new InvalidOperationException($"Environment key `{key}` has already been registered.");
            _configuration[key] = Environment.GetEnvironmentVariable(key) ?? throw new KeyNotFoundException($"Environment key `{key}` doesn't have a value.");
        }

        return this;
    }

    public ConfigurationManager Builder()
    {
        return new ConfigurationManager(_configuration);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _configuration.GetEnumerator();
    }

    internal T? Get<T>(string key) where T : class
    {
        if (key is null) throw new ArgumentNullException("key");
        if (!_configuration.ContainsKey(key)) throw new InvalidOperationException($"Configuration value for key `{key}`");
        
        return _configuration.ContainsKey(key) ? (T)_configuration[key] : null;
    }

    internal void WithSetting(object name, object value)
    {
        if (name is null) throw new ArgumentNullException("name");
        _configuration[name.ToString()!] = value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
