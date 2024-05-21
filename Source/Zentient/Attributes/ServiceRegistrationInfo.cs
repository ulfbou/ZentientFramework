
using Microsoft.Extensions.DependencyInjection;

namespace Zentient.Attributes.Injections
{
    internal class ServiceRegistrationInfo
    {
        public ServiceRegistrationInfo(Type serviceType, Type implementationType, string? serviceName, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ServiceName = serviceName;
            Lifetime = lifetime;
        }

        public Type ImplementationType { get; internal set; }
        public string ServiceName { get; internal set; }
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
    }
}