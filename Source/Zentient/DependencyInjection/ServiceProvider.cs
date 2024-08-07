

namespace Zentient.DependencyInjection
{
    public class ServiceProvider : IServiceProvider, IAsyncDisposable
    {
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors;
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, object> _scopedInstances = new();
        private readonly IServiceProvider? _parentProvider;
        private readonly List<object> _disposables = new();

        public ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProvider? parentProvider = null)
        {
            ArgumentNullException.ThrowIfNull(serviceDescriptors);

            _serviceDescriptors = serviceDescriptors.ToDictionary(x => x.ServiceType, x => x);
            _parentProvider = parentProvider;
        }

        public ServiceProvider(Dictionary<Type, ServiceDescriptor> serviceDescriptors, ServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceDescriptors);

            _serviceDescriptors = serviceDescriptors.ToDictionary();
            _parentProvider = serviceProvider;
        }

        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        public object GetService(Type serviceType)
        {
            var service = GetCachedService(serviceType);
            if (service != null) return service;

            var descriptor = _serviceDescriptors[serviceType];
            var implementation = CreateInstance(descriptor.ImplementationType);

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    _singletonInstances[serviceType] = implementation;
                    break;
                case ServiceLifetime.Scoped:
                    _scopedInstances[serviceType] = implementation;
                    break;
            }

            return implementation;
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            return (TService)await GetServiceAsync(typeof(TService));
        }

        public async Task<object> GetServiceAsync(Type serviceType)
        {
            var service = await GetServiceAsync(serviceType);
            if (service != null) return service;

            var descriptor = _serviceDescriptors[serviceType];
            var implementation = await CreateInstanceAsync(descriptor.ImplementationType);

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    _singletonInstances[serviceType] = implementation;
                    break;
                case ServiceLifetime.Scoped:
                    _scopedInstances[serviceType] = implementation;
                    break;
            }

            TrackDisposable(implementation);

            return implementation;
        }

        private object? GetCachedService(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            if (_singletonInstances.ContainsKey(serviceType))
            {
                return _singletonInstances[serviceType];
            }

            if (_scopedInstances.ContainsKey(serviceType))
            {
                return _scopedInstances[serviceType];
            }

            if (!_serviceDescriptors.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
            }

            return null;
        }

        public ServiceProvider CreateScope()
        {
            return new ServiceProvider(_serviceDescriptors, this);
        }

        private object CreateInstance(Type implementationType)
        {
            // TODO: Evaluate all potential constructors and select the one with the most parameters that can be instantiated.
            var constructor = implementationType.GetConstructors().First();
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                try
                {
                    // TODO: Handle null return value.
                    return Activator.CreateInstance(implementationType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create instance of type {implementationType}.", ex);
                }
            }
            try
            {
                // TODO: Handle null return value.
                var parameterInstances = parameters.Select(param => GetService(param.ParameterType)).ToArray();
                return Activator.CreateInstance(implementationType, parameterInstances);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create instance of type {implementationType}.", ex);
            }
        }


        private async Task<object> CreateInstanceAsync(Type implementationType)
        {
            var constructor = implementationType.GetConstructors().First();
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(implementationType);
            }

            var parameterInstances = await Task.WhenAll(parameters.Select(async param => await GetServiceAsync(param.ParameterType)));
            return Activator.CreateInstance(implementationType, parameterInstances);
        }

        private void TrackDisposable(object implementation)
        {
            if (implementation is IDisposable || implementation is IAsyncDisposable)
            {
                _disposables.Add(implementation);
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables.OfType<IDisposable>())
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var disposable in _disposables.OfType<IAsyncDisposable>())
            {
                await disposable.DisposeAsync();
            }

            Dispose();

            _disposables.Clear();
        }
    }
}