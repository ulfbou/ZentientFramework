using Microsoft.Extensions.DependencyInjection;

namespace Zentient.Repository
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepository<TRepository>(this IServiceCollection container)
        {
            // Generate DI for unit of work
            container.AddSingleton(typeof(IRepository<,>), typeof(TRepository));
            container.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
            return container;
        }
    }
}
