

using System.Reflection.Metadata;
using Zentient.Configurator.Services;
using Zentient.Configurator.Contexts;

namespace Zentient.Configurator
{
    /// <summary>
    /// Handles the initial setup and installation of default configurations and schemas.
    /// </summary>
    public class ConfigurationInstaller
    {
        private readonly ConfigurationService _configurationService;

        public ConfigurationInstaller(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void Install()
        {
            _configurationService.CreateSchema(builder =>
                builder.WithName("DefaultSchema")
                       .WithProperty("Key1", "string")
                       .WithProperty("Key2", "int"));

            _configurationService.CreateConfiguration(builder =>
                builder.WithSchema("DefaultSchema")
                       .WithProperty("Key1", "Value1")
                       .WithProperty("Key2", "123"));
        }
    }
}
