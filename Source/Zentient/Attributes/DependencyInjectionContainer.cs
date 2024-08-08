using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Zentient.Attributes;

public class DependencyInjectionContainer : IDisposable
{
    /// <summary>
    /// Dictionary storing service types and their corresponding registration information.
    /// </summary>
    private Dictionary<Type, List<ServiceRegistrationInfo>> _serviceTypes = new Dictionary<Type, List<ServiceRegistrationInfo>>();

    /// <summary>
    /// Dictionary storing factory functions for creating instances of specified types.
    /// </summary>
    private Dictionary<Type, Func<object>> _factoryRegister = new Dictionary<Type, Func<object>>();

    /// <summary>
    /// Dictionary storing configuration settings.
    /// </summary>
    private Dictionary<string, string> _config = new Dictionary<string, string>();

    /// <summary>
    ///  Bool remembering if class has been disposed. 
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Gets the current environment from the ASP.NET Core environment variable.
    /// </summary>
    public string? CurrentEnvironment
    {
        get => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    #region configuration methods
    /// <summary>
    /// Initializes the configuration settings within the dependency injection container.
    /// </summary>
    /// <param name="configuration">The IConfiguration object containing configuration settings.</param>
    public void InitializeConfig(IConfiguration configuration)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));

        _config = configuration.GetChildren()
            .SelectMany(section => section.AsEnumerable(), (section, kvp) => new KeyValuePair<string, string>($"{section.Path}:{kvp.Key}", kvp.Value))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var configSection in configuration.GetChildren())
        {
            foreach (var kvp in configSection.AsEnumerable())
            {
                _config[$"{configSection.Path}:{kvp.Key}"] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// Injects configuration settings into the specified object instance using the provided configurator function.
    /// </summary>
    /// <param name="instance">The object instance to be configured.</param>
    /// <param name="configurator">The configurator function responsible for applying the configuration settings to the instance.</param>
    /// <param name="serviceName">Optional. The name of the service to which the configuration applies.</param>
    public void InjectConfiguration(object instance, Action<object> configurator, string serviceName = null)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));

        if (instance is null) throw new ArgumentNullException(nameof(instance));
        if (configurator is null) throw new ArgumentNullException(nameof(configurator));

        Type instanceType = instance.GetType();

        try
        {
            // Invoke the configurator action with the implementation instance
            configurator(instance);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Configuration of '{instanceType.Name}' failed for registration '{instance.GetType().Name}': {ex.Message}", ex);
        }

        IEnumerable<ServiceRegistrationInfo> registrationInfos = _serviceTypes
            .Where(kv => kv.Value.Any(info => info.ImplementationType == instanceType))
            .SelectMany(kvp => kvp.Value);

        // Check if the instance's type has been registered
        if (registrationInfos.Count() == 0)
        {
            throw new InvalidOperationException($"Type '{instanceType.Name}' has not been registered for configuration injection. Make sure to register the type before injecting configuration.");
        }

        // If a service name is specified, filter registrationInfos based on the service name
        if (!string.IsNullOrEmpty(serviceName))
        {
            registrationInfos = registrationInfos.Where(info => info.ServiceName == serviceName).ToList();
        }

        foreach (var registrationInfo in registrationInfos)
        {
            try
            {
                // Create an instance of the implementation type
                var implementationInstance = Activator.CreateInstance(registrationInfo.ImplementationType);

                // Invoke the configurator action with the implementation instance
                configurator(implementationInstance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Configuration of '{instanceType.Name}' failed for registration '{registrationInfo.ServiceName}': {ex.Message}", ex);
            }
        }
    }

#if MAYBE_LATER
    public void InjectConfiguration(object instance, IConfiguration configuration, params string[] serviceNames)
    {
        // Inject configuration settings for specific service names
    }
    public void InjectConfiguration(object instance, IConfiguration configuration, ConfigurationScope scope)
    {
        // Inject configuration settings based on the specified scope (e.g., Global, PerRequest, PerEnvironment)
    }
    public void InjectConfiguration<TConfig>(object instance, TConfig configuration) where TConfig : class
    {
        // Inject strongly typed configuration object
    }
    public void InjectConfiguration(object instance, IConfiguration configuration, IConfiguration defaultConfiguration)
    {
        // Inject configuration settings, falling back to defaultConfiguration for any missing settings
    }
public void InjectConfiguration<TService>(object instance, IConfiguration configuration) where TService : class
{
// Inject configuration settings for services of type TService
}
public void InjectConfiguration(object instance, IConfiguration configuration, Func<string, bool> filter)
{
// Inject configuration settings based on the provided filter predicate
}
public void InjectConfiguration(object instance, IConfiguration configuration, params string[] sections)
{
// Inject configuration settings for specified configuration sections
}
public void InjectConfiguration(object instance, params IConfiguration[] configurations)
{
// Inject configuration settings, merging settings from multiple IConfiguration sources
}

public enum ConfigurationScope
{
Global,
PerRequest,
PerEnvironment,
// Add more scope options as needed
}
#endif

    #endregion

    #region registration methods
    /// <summary>
    /// Registers a factory function that produces instances of a specified type.
    /// </summary>
    /// <param name="serviceType">The type of object to be produced by the factory.</param>
    /// <param name="factory">The factory function responsible for creating instances of the specified type.</param>
    /// <param name="serviceName">Optional. The name or alias used to identify specific instances or variations of the service (if multiple registrations of the same type exist).</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration, determining how long created instances will live (defaults to Transient).</param>
    public void RegisterFactory(Type serviceType, Func<object> factory, string? serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));
        if (!_serviceTypes.ContainsKey(serviceType))
        {
            _serviceTypes[serviceType] = new List<ServiceRegistrationInfo>();
        }

        object instance = factory();

        if (instance is null) throw new InvalidOperationException($"Factory delegate `{nameof(factory)}` failed to produce an instance.");

        Type implementationType = instance.GetType();

        // Add registration information to the list associated with the type
        ServiceRegistrationInfo registrationInfo = new ServiceRegistrationInfo(serviceType, implementationType, serviceName, lifetime);
        _serviceTypes[serviceType].Add(registrationInfo);
    }

    /// <summary>
    /// Registers a factory function that produces instances of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to be produced by the factory.</typeparam>
    /// <param name="factory">The factory function responsible for creating instances of the specified type.</param>
    /// <param name = "serviceName" > Optional.The name or alias of the service to be registered.</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration (defaults to Transient).</param>
    public void RegisterFactory<T>(Func<object> factory, string? serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Transient) => RegisterFactory(typeof(T), factory, serviceName, lifetime);

    /// <summary>
    /// Registers a service type with its corresponding implementation type in the dependency injection container.
    /// </summary>
    /// <param name="serviceType">The type of the service to be registered.</param>
    /// <param name="implementationType">The type implementing the service to be registered.</param>
    /// <param name="serviceName">Optional. The name or alias used to identify specific instances or variations of the service (if multiple registrations of the same type exist).</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration, determining how long created instances will live (defaults to Transient).</param>
    public void RegisterService(Type serviceType, Type implementationType, string? serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));

        // Perform service exclusion and configuration check based on implementationType
        if (IsServiceExcludedForCurrentEnvironment(implementationType) || IsServiceDisabledBasedOnConfiguration(implementationType))
        {
            return;
        }

        // Create service registration information
        // Why implementationType as serviceType?
        ServiceRegistrationInfo registrationInfo = new ServiceRegistrationInfo(implementationType, implementationType, serviceName, lifetime);

        // Initialize list if serviceType is not already registered
        if (!_serviceTypes.ContainsKey(serviceType))
        {
            _serviceTypes[serviceType] = new List<ServiceRegistrationInfo>();
        }

        // Add registration information to the list associated with the serviceType
        _serviceTypes[serviceType].Add(registrationInfo);
    }

    /// <summary>
    /// Registers a service mapping from one type to another in the dependency injection container.
    /// </summary>
    /// <typeparam name="TFrom">The type of service to be registered.</typeparam>
    /// <typeparam name="TTo">The type implementing the service to be registered.</typeparam>
    /// <param name="serviceName">Optional. The name or alias used to identify specific instances or variations of the service (if multiple registrations of the same type exist).</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration, determining how long created instances will live (defaults to Transient).</param>
    public void RegisterService<TFrom, TTo>(string? serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Transient) => RegisterService(typeof(TFrom), typeof(TTo), serviceName, lifetime);

    /// <summary>
    /// Registers a service type with its corresponding implementation type in the dependency injection container.
    /// </summary>
    /// <typeparam name="TService">The type of the service to be registered.</typeparam>
    /// <param name="implementationType">The type implementing the service to be registered.</param>
    /// <param name="serviceName">Optional. The name or alias used to identify specific instances or variations of the service (if multiple registrations of the same type exist).</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration, determining how long created instances will live (defaults to Transient).</param>
    public void RegisterService<TService>(Type implementationType, string? serviceName, ServiceLifetime lifetime = ServiceLifetime.Transient) => RegisterService(typeof(TService), implementationType, serviceName, lifetime);

    /// <summary>
    /// Registers services from types found in the specified assembly in the dependency injection container.
    /// </summary>
    /// <param name="assembly">The assembly containing types to be registered as services.</param>
    /// <param name="lifetime">Optional. The lifetime of the service registration, determining how long created instances will live (defaults to Transient).</param>
    public void RegisterServices(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var attributeTypes = assembly.GetTypes()
            .Where(type => type.Namespace == "Zentient.Attributes");
        var typesWithConfigurationBasedSelector = attributeTypes
            .Where(type => type.GetCustomAttribute<ConfigurationBasedSelectorAttribute>() != null);

        // Class/Method: ConfigurationKey
        foreach (var type in typesWithConfigurationBasedSelector)
        {
            RegisterService(type, type, lifetime: lifetime);
        }

        var typesWithEnvironmentBasedSelector = attributeTypes
            .Where(type => type.GetCustomAttribute<EnvironmentSelectorAttribute>() != null);

        // Class/Method: EnvironmentName
        foreach (var type in typesWithEnvironmentBasedSelector)
        {
            RegisterService(type, type, lifetime: lifetime);
        }

        var typesWithInjectConfigurationAttribute = attributeTypes
            .Where(type => type.GetCustomAttribute<InjectConfigurationAttribute>() != null);

        // Constructor/Property
        foreach (var type in typesWithInjectConfigurationAttribute)
        {
            RegisterService(type, type, lifetime: lifetime);
        }

        var typesWithRegisterServiceAttribute = attributeTypes
            .Where(type => type.GetCustomAttribute<RegisterServiceAttribute>() != null);

        // Class: ServiceType
        foreach (var type in typesWithRegisterServiceAttribute)
        {
            var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
            RegisterService(attribute.ServiceType, type, lifetime: lifetime);
        }

        var methodsWithRegisterFactoryAttribute = attributeTypes
            .SelectMany(type => type.GetMethods())
            .Where(method => method.GetCustomAttribute<RegisterFactoryAttribute>() != null);

        // Class: ServiceType
        foreach (var method in methodsWithRegisterFactoryAttribute)
        {
            var attribute = method.GetCustomAttribute<RegisterFactoryAttribute>();
            var serviceType = attribute.ServiceType;
            var factory = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), method);
            RegisterFactory(serviceType, factory, lifetime: lifetime);
        }
    }



#if WRONG
namespace Zentient.Attributes
{
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ConfigurationBasedSelectorAttribute
{
string ConfigurationKey { get; set; }
}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EnvironmentSelectorAttribute : Attribute
{
public string EnvironmentName {get;set;}
public EnvironmentSelectorMode Mode { get; set; }
}
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
public class InjectConfigurationAttribute : Attribute {}
[AttributeUsage(AttributeTargets.Class)]
public class RegisterServiceAttribute : Attribute, IRegisterServiceAttribute
{
public Type ServiceType { get; }
}
[AttributeUsage(AttributeTargets.Method)]
public class RegisterFactoryAttribute : Attribute
{
public Type ServiceType { get; }
}
}

    public void RegisterServices(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));

        var types = assembly.GetExportedTypes();

        foreach (var type in types)
        {
            if (IsServiceExcludedForCurrentEnvironment(type) || IsServiceDisabledBasedOnConfiguration(type))
            {
                continue;
            }

            RegisterService(type, type, lifetime: lifetime);
        }
    }
#endif


    #endregion

    #region retrieval methods
    /// <summary>
    /// Retrieves an instance of the specified service type.
    /// </summary>
    /// <param name="type">The type of service to retrieve.</param>
    /// <returns>An instance of the specified service type.</returns>
    public object GetService(Type type)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));
        if (type is null) throw new ArgumentNullException(nameof(type));

        object? instance = null;

        if (_factoryRegister.ContainsKey(type))
        {
            Func<object> factory = _factoryRegister[type];

            try
            {
                instance = factory();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Service factory '{factory.GetType().Name}' could not produce '{type.Name}'.", ex);
            }
        }
        else if (_serviceTypes.ContainsKey(type))
        {
            // TODO: We need to handle lifetime
            instance = CreateServiceInstance(type);
        }
        else
        {
            throw new InvalidOperationException($"Neither registration or factory could be found for service '{type.Name}'.");
        }

        return instance!;
    }

    /// <summary>
    /// Retrieves an instance of the specified service type.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    public T GetService<T>() => (T)GetService(typeof(T));

    /// <summary>
    /// Retrieves an instance of the specified service type with the specified service name.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="serviceName">The name of the service to retrieve.</param>
    /// <returns>An instance of the specified service type.</returns>
    public T GetService<T>(string serviceName)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));
        if (string.IsNullOrEmpty(serviceName)) throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));

        Type serviceType = typeof(T);
        object instance = GetService(serviceType, serviceName);

        if (instance != null && instance is T serviceInstance)
        {
            return serviceInstance;
        }
        else
        {
            throw new InvalidOperationException($"Service of type '{typeof(T).Name}' with name '{serviceName}' could not be found or created.");
        }
    }

    public object GetService(Type type, string serviceName)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));

        if (_factoryRegister.ContainsKey(type))
        {
            Func<object> factory = _factoryRegister[type];
            try
            {
                return factory();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Service factory '{factory.GetType().Name}' could not produce '{type.Name}' for service name '{serviceName}'.", ex);
            }
        }

        if (_serviceTypes.ContainsKey(type))
        {
            return CreateServiceInstance(type, serviceName);
        }

        throw new InvalidOperationException($"Neither registration nor factory could be found for service '{type.Name}' for service name '{serviceName}'.");
    }

    /// <summary>
    /// Retrieves all instances of the specified service type.
    /// </summary>
    /// <param name="type">The type of service to retrieve.</param>
    /// <returns>An enumerable collection of instances of the specified service type.</returns>
    public IEnumerable<object> GetServices(Type type)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));
        if (type is null) throw new ArgumentNullException(nameof(type));

        if (!_serviceTypes.ContainsKey(type))
        {
            return Enumerable.Empty<object>();
        }

        var registrationInfos = _serviceTypes[type];
        var instances = new List<object>();

        foreach (var registrationInfo in registrationInfos)
        {
            try
            {
                var instance = Activator.CreateInstance(registrationInfo.ImplementationType);

                if (instance is null) throw new InvalidOperationException($"Service '{type.Name}' could not be created.");

                instances.Add(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Service '{type.Name}' could not be created.", ex);
            }
        }

        return instances;
    }

    /// <summary>
    /// Retrieves all instances of the specified service type.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>An enumerable collection of instances of the specified service type.</returns>
    public IEnumerable<T> GetServices<T>() => GetServices(typeof(T)).Cast<T>();

    /// <summary>
    /// Retrieves all instances of the specified service type with the specified service name.
    /// </summary>
    /// <param name="type">The type of service to retrieve.</param>
    /// <param name="serviceName">The name of the service to retrieve.</param>
    /// <returns>An enumerable collection of instances of the specified service type.</returns>
    public IEnumerable<object> GetServices(Type type, string serviceName)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DependencyInjectionContainer));
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
        if (!_serviceTypes.ContainsKey(type)) return Enumerable.Empty<object>();

        var registrationInfos = _serviceTypes[type];

        // Filter registrationInfos based on the service name
        registrationInfos = registrationInfos.Where(info => info.ServiceName == serviceName).ToList();

        var instances = new List<object>();

        foreach (var registrationInfo in registrationInfos)
        {
            try
            {
                var instance = Activator.CreateInstance(registrationInfo.ImplementationType);

                if (instance is null) throw new InvalidOperationException($"Service '{type.Name}' could not be created.");

                instances.Add(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Service '{type.Name}' could not be created.", ex);
            }
        }

        return instances;
    }

    /// <summary>
    /// Retrieves all instances of the specified service type with the specified service name.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="serviceName">The name of the service to retrieve.</param>
    /// <returns>An enumerable collection of instances of the specified service type.</returns>
    public IEnumerable<T> GetServices<T>(string serviceName) => GetServices(typeof(T), serviceName).Cast<T>();
    #endregion

    #region disposable methods
    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations before the object is reclaimed by garbage collection.
    /// </summary>
    public void AsyncDispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Perform managed resource cleanup here
            _serviceTypes.Clear();
            _factoryRegister.Clear();
            _config.Clear();

            _serviceTypes = null!;
            _factoryRegister = null!;
            _config = null!;
        }

        _disposed = true;
    }
    #endregion

    #region private methods
    private object? CreateServiceInstance(Type type)
    {       // TODO: Doesn't work 
        try
        {
            object? instance = null;
            IEnumerable<ServiceRegistrationInfo> registrationInfos = _serviceTypes[type];

            var registrationInfo = registrationInfos
                .FirstOrDefault(info =>
                {
                    if (info.ImplementationType is null) return false;
                    try
                    { //TODO: keep track of which classes need parameters
                        instance = Activator.CreateInstance(info.ImplementationType);
                        return true;
                    }
                    catch
                    {
                        instance = null;
                        return false;
                    }
                });

            return instance;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Service '{type.Name}' could not be created.", ex);
        }
    }

    private object CreateServiceInstance(Type type, string serviceName)
    {
        if (_serviceTypes.TryGetValue(type, out List<ServiceRegistrationInfo>? registrationInfos))
        {
            var registrationInfo = registrationInfos
                .FirstOrDefault(info => info.ServiceName == serviceName && info.ImplementationType != null);

            if (registrationInfo != null)
            {
                try
                {
                    return Activator.CreateInstance(registrationInfo.ImplementationType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Service '{type.Name}' could not be created for service name '{serviceName}'.", ex);
                }
            }
        }

        throw new InvalidOperationException($"No registration or factory found for service '{type.Name}' for service name '{serviceName}'.");
    }

    /// <summary>
    /// Determines if a service is excluded for the current environment based on attributes.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>True if the service is excluded for the current environment; otherwise, false.</returns>
    private bool IsServiceExcludedForCurrentEnvironment(Type implementationType)
    {
        // Update parameter name for clarity
        var environmentSelectorAttribute = implementationType.GetCustomAttribute<EnvironmentSelectorAttribute>(true);

        // Check if environmentSelectorAttribute is null to determine if service is excluded based on environment
        if (environmentSelectorAttribute == null)
        {
            return false;
        }

        return environmentSelectorAttribute.Mode == EnvironmentSelectorMode.Exclude &&
               CurrentEnvironment == environmentSelectorAttribute.EnvironmentName;
    }

    /// <summary>
    /// Determines if a service is disabled based on configuration settings.
    /// </summary>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>True if the service is disabled based on configuration; otherwise, false.</returns>
    private bool IsServiceDisabledBasedOnConfiguration(Type implementationType)
    {
        string configurationKey = $"{implementationType.Name}:Disabled";

        try
        {
            if (_config.TryGetValue(configurationKey, out string disabledValue))
            {
                return disabledValue == "true";
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions if configuration retrieval fails
            // Log or handle the exception accordingly
            Console.WriteLine($"Error retrieving configuration key '{configurationKey}': {ex.Message}");
        }

        return false; // Default to enabled if configuration retrieval fails or key is not found
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DependencyInjectionContainer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
