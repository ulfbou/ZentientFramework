using System.Collections;

namespace Zentient.Configuration;

public class Configurator : IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _configuration = new Dictionary<string, object>();

    public Configurator Set(string key, object value)
    {
        _configuration[key] = value;
        return this;
    }

    public T Get<T>(string key)
    {
        if (_configuration.ContainsKey(key))
        {
            return (T)_configuration[key];
        }

        throw new KeyNotFoundException($"The configuration key '{key}' was not found.");
    }

    public void Load(Action<Configurator> configurationAction)
    {
        configurationAction(this);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, object>>)_configuration).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_configuration).GetEnumerator();
    }
}
