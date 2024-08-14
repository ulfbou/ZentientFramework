namespace Zentient.DependencyInjection
{
    public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors = new();

        public IServiceCollection AddSingleton<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
            return this;
        }

        public IServiceCollection AddScoped<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
            return this;
        }

        public IServiceCollection AddTransient<TService, TImplementation>() where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
            return this;
        }

        public IServiceProvider Build()
        {
            return new ServiceProvider(_serviceDescriptors);
        }
    }
}
