namespace Zentient.DependencyInjection
{
    public class ServiceScope : IServiceScope
    {
        private readonly ServiceProvider _serviceProvider;

        public ServiceScope(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public ValueTask DisposeAsync() => _serviceProvider.DisposeAsync();
    }
}