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

        public static IServiceCollection AddRepository(this IServiceCollection container, Type? repositoryType = null)
        {
            if (repositoryType is null) repositoryType = typeof(RepositoryBase<,>);

            container.AddSingleton(typeof(IRepository<,>), repositoryType);
            container.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
            return container;
        }
    }
}
