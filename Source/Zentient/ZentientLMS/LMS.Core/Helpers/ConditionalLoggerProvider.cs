using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LMS.Core.Helpers
{
    public class ConditionalLoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ConditionalLoggerProvider(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ILogger<T>? GetLogger<T>()
        {
            if (!string.IsNullOrWhiteSpace(_configuration["UseConditionalLogger"]) || _configuration["UseConditionalLogger"] == "false")
            {
                return null;
            }

            IConfigurationSection providers = _configuration.GetSection("LoggerProviders");

            try
            {
                // Generate code to test if Environment.GetDevelopmentEnvironment() exists in providers and if so, attempt to get the required serivce. If it fails, return null.
                var env = Environment.GetDevelopmentEnvironment();

                if (env == null || providers[env] == null)
                {
                    return null;
                }

                // TODO: get the logger from the service provider and 
                // return it. If it fails, return null.
                // 

                if (Environment.IsDevelopment())
                {
                    // Use a specific logger for development
                    return _serviceProvider.GetRequiredService<ILogger<T>>();
                }
                else
                {
                    // TODO: Use a different logger for other environments
                    return _serviceProvider.GetRequiredService<ILogger<T>>();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
