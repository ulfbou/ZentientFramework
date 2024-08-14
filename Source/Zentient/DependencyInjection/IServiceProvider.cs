
namespace Zentient.DependencyInjection
{
    public interface IServiceProvider : IDisposable
    {
        IServiceProvider CreateScope();
        TService GetService<TService>();
        object GetService(Type serviceType);
        Task<TService> GetServiceAsync<TService>();
        Task<object> GetServiceAsync(Type serviceType);
        Dictionary<Type, ServiceDescriptor> ServiceDescriptors { get; }
    }
}