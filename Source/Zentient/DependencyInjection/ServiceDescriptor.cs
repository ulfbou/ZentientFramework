namespace Zentient.DependencyInjection
{
    public class ServiceDescriptor
    {
        private readonly Type _serviceType;
        private readonly Type _implementationType;
        private readonly ServiceLifetime _lifetime;

        public Type ServiceType => _serviceType;
        public Type ImplementationType => _implementationType;
        public ServiceLifetime Lifetime => _lifetime;

        public ServiceDescriptor(
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            _lifetime = lifetime;
        }
    }
}