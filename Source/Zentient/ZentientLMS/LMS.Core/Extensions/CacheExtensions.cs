using LMS.Core.Domain.Dispatching;
using LMS.Core.Domain.Events;
using LMS.Core.Domain.Events.Contracts;
using LMS.Core.Handlers.Caching;
using LMS.Core.Infrastructure.Caching;
using LMS.Core.Services.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Core.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<DataUpdatedEvent>, CacheInvalidationHandler>();
            // Register other services and handlers as needed

            return services;
        }

        public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
        {
            services.AddSingleton<DomainEventDispatcher>();
            return services;
        }

        public static IServiceCollection AddDomainEventHandler<TDomainEvent, TDomainEventHandler>(this IServiceCollection services)
            where TDomainEvent : IDomainEvent
            where TDomainEventHandler : class, IDomainEventHandler<TDomainEvent>
        {
            services.AddSingleton<IDomainEventHandler<TDomainEvent>, TDomainEventHandler>();
            return services;
        }

        public static IServiceCollection AddCacheInvalidationHandler(this IServiceCollection services)
        {
            services.AddDomainEventHandler<DataUpdatedEvent, CacheInvalidationHandler>();
            return services;
        }

        public static IServiceCollection AddExpirableCache<TKey, TValue>(this IServiceCollection services)
        {
            services.AddSingleton<ExpirableCache<TKey, TValue>>();
            return services;
        }

        public static IServiceCollection AddDataService(this IServiceCollection services)
        {
            services.AddSingleton<DataService>();
            return services;
        }

        public static IServiceCollection AddBetterDataService(this IServiceCollection services)
        {
            services.AddSingleton<BetterDataService>();
            return services;
        }
    }
}
