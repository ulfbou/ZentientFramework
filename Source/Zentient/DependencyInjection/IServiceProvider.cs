namespace Zentient.DependencyInjection
{
    public interface IServiceProvider
    {
        TService GetService<TService>();
        object GetService(Type serviceType);
        Task<TService> GetServiceAsync<TService>();
        Task<object> GetServiceAsync(Type serviceType);
    }
}