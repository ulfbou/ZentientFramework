namespace Zentient.DependencyInjection
{
    public class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _rootProvider;

        public ServiceScopeFactory(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public IServiceScope CreateScope()
        {
            return new ServiceScope(new ServiceProvider(_rootProvider.ServiceDescriptors, _rootProvider));
        }
    }
}