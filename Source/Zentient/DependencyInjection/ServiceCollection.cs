namespace Zentient.DependencyInjection
{
    public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors = new();

        public void AddSingleton<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
        }

        public void AddScoped<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
        }

        public void AddTransient<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
        }

        public IServiceProvider Build()
        {
            return new ServiceProvider(_serviceDescriptors);
        }
    }
}
