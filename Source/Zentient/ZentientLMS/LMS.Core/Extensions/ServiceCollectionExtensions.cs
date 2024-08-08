using LMS.Core.Infrastructure.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var httpClientConfigs = configuration.GetSection("HttpClients").GetChildren() ??
                throw new InvalidOperationException("No configuration section 'HttpClients' found.");

            foreach (var config in httpClientConfigs)
            {
                var clientName = config.Key;
                var httpClientConfig = config.Get<HttpClientConfig>();

                if (httpClientConfig == null)
                {
                    throw new InvalidOperationException($"Configuration for HttpClient '{clientName}' not found.");
                }

                services.AddHttpClient(clientName, client =>
                {
                    client.BaseAddress = new Uri(httpClientConfig.BaseAddress ??
                        throw new InvalidOperationException("Base address not found in configuration."));

                    foreach (var header in httpClientConfig.DefaultRequestHeaders ??
                        throw new InvalidOperationException("Endpoint path not found in configuration."))
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                });
            }

            return services;
        }

        public static IServiceCollection AddRequestService<TService, TImplementation>(this IServiceCollection services, IConfiguration configuration)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            string serviceName = typeof(TService).Name;
            IConfigurationSection requestServiceConfig = configuration.GetSection($"RequestServices:{serviceName}");

            var baseAddress = requestServiceConfig.GetValue<string>("BaseAddress") ?? throw new InvalidOperationException($"Base address not found for service '{serviceName}'.");

            services.AddHttpClient<TService, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                var defaultRequestHeaders = requestServiceConfig.GetSection("DefaultRequestHeaders").Get<Dictionary<string, string>>();
                foreach (var header in defaultRequestHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }).Services.AddScoped<TService, TImplementation>(serviceProvider =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(TService).Name);
                return Activator.CreateInstance(typeof(TImplementation), httpClient, configuration, serviceName) as TImplementation ?? throw new InvalidOperationException($"Could not create instance of {typeof(TImplementation).Name}.");
            });

            return services;
        }
    }
}
