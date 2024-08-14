namespace Zentient.DependencyInjection
{
    public class ServiceProvider : IServiceProvider, IAsyncDisposable
    {
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors;
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, object> _scopedInstances = new();
        private readonly IServiceProvider? _parentProvider;
        private readonly List<object> _disposables = new();

        public ServiceProvider(Dictionary<Type, ServiceDescriptor> serviceDescriptorsDictionary, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceDescriptorsDictionary);

            _serviceDescriptors = serviceDescriptorsDictionary.ToDictionary();
            _parentProvider = serviceProvider;
        }

        public ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProvider? parentProvider = null)
        {
            ArgumentNullException.ThrowIfNull(serviceDescriptors);

            _serviceDescriptors = serviceDescriptors.ToDictionary(x => x.ServiceType, x => x);
            _parentProvider = parentProvider;
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

            TrackDisposable(implementation);

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

        public IServiceProvider CreateScope()
        {
            return new ServiceProvider(_serviceDescriptors, this);
        }

        private object CreateInstance(Type implementationType)
        {
            // TODO: Evaluate all potential constructors and select the one with the most parameters that can be instantiated.
            var constructor = ConstructorSelector.GetBestConstructor(implementationType, this);
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(implementationType);
            }

            var parameterInstances = new List<object>();
            foreach (var param in parameters)
            {
                if (CanResolve(param.ParameterType))
                {
                    parameterInstances.Add(GetServiceAsync(param.ParameterType));
                }
                else if (param.HasDefaultValue)
                {
                    parameterInstances.Add(param.DefaultValue);
                }
                else
                {
                    throw new InvalidOperationException($"Unable to resolve parameter '{param.Name}' for type '{implementationType.FullName}'");
                }
            }

            return Activator.CreateInstance(implementationType, parameterInstances.ToArray());
        }

        private async Task<object> CreateInstanceAsync(Type implementationType)
        {
            var constructor = ConstructorSelector.GetBestConstructor(implementationType, this);
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(implementationType);
            }

            var parameterInstances = new List<object>();
            foreach (var param in parameters)
            {
                if (CanResolve(param.ParameterType))
                {
                    parameterInstances.Add(GetServiceAsync(param.ParameterType));
                }
                else if (param.HasDefaultValue)
                {
                    parameterInstances.Add(param.DefaultValue);
                }
                else
                {
                    throw new InvalidOperationException($"Unable to resolve parameter '{param.Name}' for type '{implementationType.FullName}'");
                }
            }

            return Activator.CreateInstance(implementationType, parameterInstances.ToArray());
        }

        private bool CanResolve(Type type)
        {
            try
            {
                GetServiceAsync(type).Wait();
                return true;
            }
            catch
            {
                return false;
            }
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

        public Dictionary<Type, ServiceDescriptor> ServiceDescriptors => _serviceDescriptors;
    }
}