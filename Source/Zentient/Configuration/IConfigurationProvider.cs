namespace Zentient.Configuration;

public interface IConfigurationProvider
{
    ConfigurationBuilder Load();
}
