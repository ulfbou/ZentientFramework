# ⚙️ Configuration Management Architecture

## Overview

Zentient.Abstractions provides a sophisticated configuration management system that goes beyond simple key-value settings. It offers strongly-typed configuration definitions, hierarchical configuration scopes, dynamic configuration updates, and comprehensive validation. The system integrates seamlessly with the envelope pattern for consistent error handling and provides rich metadata for configuration sources and changes.

## 🎯 Core Concepts

### **Configuration Definitions as Architecture**

Configurations are not just settings - they are architectural components with rich metadata and validation:

```csharp
[ConfigurationDefinition("Database", Version = "2.1.0")]
public record DatabaseConfiguration : IConfigurationDefinition<DatabaseConfiguration>
{
    public string Id => "DatabaseConfig.v2.1";
    public string Name => "Database Configuration";
    public string Description => "Database connection and performance settings";
    
    // Configuration properties with validation
    [Required(ErrorMessage = "Connection string is required")]
    [MinLength(10, ErrorMessage = "Connection string must be at least 10 characters")]
    public string ConnectionString { get; init; } = string.Empty;
    
    [Range(1, 1000, ErrorMessage = "Max connections must be between 1 and 1000")]
    public int MaxConnections { get; init; } = 100;
    
    [Range(1, 300, ErrorMessage = "Command timeout must be between 1 and 300 seconds")]
    public int CommandTimeoutSeconds { get; init; } = 30;
    
    [Range(0, 10, ErrorMessage = "Retry count must be between 0 and 10")]
    public int RetryCount { get; init; } = 3;
    
    public bool EnableConnectionPooling { get; init; } = true;
    public bool EnableQueryLogging { get; init; } = false;
    public bool EnablePerformanceCounters { get; init; } = true;
    
    [Range(1, 3600, ErrorMessage = "Health check interval must be between 1 and 3600 seconds")]
    public int HealthCheckIntervalSeconds { get; init; } = 60;
    
    // Nested configuration
    public RetryPolicyConfiguration RetryPolicy { get; init; } = new();
    public CachingConfiguration Caching { get; init; } = new();
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "Infrastructure",
        ["Layer"] = "DataAccess",
        ["Scope"] = "Application",
        ["SensitivityLevel"] = "High",
        ["ChangeNotification"] = "Required",
        ["ValidationLevel"] = "Strict",
        ["RefreshStrategy"] = "OnChange",
        ["BackupRequired"] = true,
        ["EncryptionRequired"] = true,
        ["AuditRequired"] = true,
        ["Dependencies"] = new[] { "ConnectionStrings", "Logging", "Metrics" },
        ["AllowedEnvironments"] = new[] { "Development", "Staging", "Production" },
        ["RequiredPermissions"] = new[] { "Configuration.Database.Read", "Configuration.Database.Write" }
    };
    
    // Custom validation logic
    public ValidationResult Validate()
    {
        var errors = new List<string>();
        
        // Connection string validation
        if (!string.IsNullOrEmpty(ConnectionString))
        {
            if (!IsValidConnectionString(ConnectionString))
                errors.Add("Connection string format is invalid");
                
            if (ConnectionString.Contains("password=", StringComparison.OrdinalIgnoreCase) &&
                !ConnectionString.Contains("Encrypt=True", StringComparison.OrdinalIgnoreCase))
                errors.Add("Connection string with password must use encryption");
        }
        
        // Performance validation
        if (MaxConnections > 500 && !EnableConnectionPooling)
            errors.Add("High connection count requires connection pooling");
            
        if (CommandTimeoutSeconds < 10 && RetryCount > 0)
            errors.Add("Short timeout with retries may cause cascading failures");
            
        // Environment-specific validation
        var environment = GetCurrentEnvironment();
        if (environment == "Production")
        {
            if (EnableQueryLogging)
                errors.Add("Query logging should be disabled in production for performance");
                
            if (!EnablePerformanceCounters)
                errors.Add("Performance counters should be enabled in production for monitoring");
        }
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
    
    // Default values for different environments
    public static DatabaseConfiguration ForEnvironment(string environment)
    {
        return environment.ToLower() switch
        {
            "development" => new DatabaseConfiguration
            {
                MaxConnections = 10,
                CommandTimeoutSeconds = 60,
                EnableQueryLogging = true,
                EnablePerformanceCounters = false,
                HealthCheckIntervalSeconds = 30
            },
            "staging" => new DatabaseConfiguration
            {
                MaxConnections = 50,
                CommandTimeoutSeconds = 30,
                EnableQueryLogging = false,
                EnablePerformanceCounters = true,
                HealthCheckIntervalSeconds = 60
            },
            "production" => new DatabaseConfiguration
            {
                MaxConnections = 100,
                CommandTimeoutSeconds = 30,
                EnableQueryLogging = false,
                EnablePerformanceCounters = true,
                HealthCheckIntervalSeconds = 60
            },
            _ => new DatabaseConfiguration()
        };
    }
}

public record RetryPolicyConfiguration
{
    [Range(0, 10)]
    public int MaxAttempts { get; init; } = 3;
    
    [Range(100, 30000)]
    public int BaseDelayMilliseconds { get; init; } = 1000;
    
    public bool UseExponentialBackoff { get; init; } = true;
    public bool UseJitter { get; init; } = true;
    
    [Range(1.0, 10.0)]
    public double BackoffMultiplier { get; init; } = 2.0;
    
    [Range(1000, 300000)]
    public int MaxDelayMilliseconds { get; init; } = 30000;
}

public record CachingConfiguration
{
    public bool EnableCaching { get; init; } = true;
    
    [Range(1, 3600)]
    public int DefaultCacheDurationSeconds { get; init; } = 300;
    
    [Range(1, 10000)]
    public int MaxCacheSize { get; init; } = 1000;
    
    public bool EnableDistributedCache { get; init; } = false;
    public string? DistributedCacheConnectionString { get; init; }
}
```

### **Strongly-Typed Configuration Services**

Type-safe configuration access with automatic binding and validation:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class TypedConfigurationService : ITypedConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationValidator _validator;
    private readonly ILogger<TypedConfigurationService> _logger;
    private readonly ConcurrentDictionary<Type, object> _configurationCache;
    private readonly IChangeTokenFactory _changeTokenFactory;
    
    public TypedConfigurationService(
        IConfiguration configuration,
        IConfigurationValidator validator,
        ILogger<TypedConfigurationService> logger,
        IChangeTokenFactory changeTokenFactory)
    {
        _configuration = configuration;
        _validator = validator;
        _logger = logger;
        _changeTokenFactory = changeTokenFactory;
        _configurationCache = new ConcurrentDictionary<Type, object>();
    }
    
    public async Task<IEnvelope<T, ConfigurationCode, ConfigurationError>> GetConfigurationAsync<T>()
        where T : class, IConfigurationDefinition<T>, new()
    {
        try
        {
            var configType = typeof(T);
            var sectionName = GetSectionName<T>();
            
            // Try cache first
            if (_configurationCache.TryGetValue(configType, out var cached))
            {
                if (cached is T cachedConfig && !IsConfigurationStale(cachedConfig))
                {
                    return Envelope.Success(ConfigurationCode.Retrieved, cachedConfig);
                }
            }
            
            // Bind configuration from sources
            var config = new T();
            var section = _configuration.GetSection(sectionName);
            
            if (!section.Exists())
            {
                _logger.LogWarning("Configuration section {SectionName} not found, using defaults", sectionName);
                config = T.ForEnvironment(GetCurrentEnvironment());
            }
            else
            {
                section.Bind(config);
            }
            
            // Validate configuration
            var validationResult = await _validator.ValidateAsync(config);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors);
                _logger.LogError("Configuration validation failed for {ConfigType}: {Errors}", 
                    configType.Name, errors);
                    
                return Envelope.ValidationError<T, ConfigurationCode, ConfigurationError>(
                    ConfigurationError.ValidationFailed(configType.Name, validationResult.Errors));
            }
            
            // Cache the validated configuration
            _configurationCache.AddOrUpdate(configType, config, (k, v) => config);
            
            // Set up change notification
            var changeToken = _changeTokenFactory.CreateChangeToken(sectionName);
            changeToken.RegisterChangeCallback(_ => InvalidateCache(configType), null);
            
            _logger.LogInformation("Successfully loaded configuration for {ConfigType}", configType.Name);
            
            return Envelope.Success(ConfigurationCode.Retrieved, config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration for {ConfigType}", typeof(T).Name);
            return Envelope.Error<T, ConfigurationCode, ConfigurationError>(
                ConfigurationError.LoadFailed(typeof(T).Name, ex.Message));
        }
    }
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> UpdateConfigurationAsync<T>(
        T configuration,
        ConfigurationScope scope = ConfigurationScope.Application)
        where T : class, IConfigurationDefinition<T>
    {
        try
        {
            // Validate before updating
            var validationResult = await _validator.ValidateAsync(configuration);
            if (!validationResult.IsValid)
            {
                return Envelope.ValidationError<ConfigurationCode, ConfigurationError>(
                    ConfigurationError.ValidationFailed(typeof(T).Name, validationResult.Errors));
            }
            
            var sectionName = GetSectionName<T>();
            var configProvider = GetConfigurationProvider(scope);
            
            // Update configuration
            await configProvider.SetAsync(sectionName, configuration);
            
            // Invalidate cache
            InvalidateCache(typeof(T));
            
            // Publish configuration change event
            var changeEvent = new ConfigurationChangedEvent
            {
                ConfigurationType = typeof(T).Name,
                SectionName = sectionName,
                Scope = scope,
                ChangedBy = GetCurrentUser(),
                Timestamp = DateTime.UtcNow,
                ChangeReason = "Manual Update"
            };
            
            await _eventPublisher.PublishAsync(changeEvent);
            
            _logger.LogInformation("Successfully updated configuration for {ConfigType} in scope {Scope}", 
                typeof(T).Name, scope);
                
            return Envelope.Success(ConfigurationCode.Updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration for {ConfigType}", typeof(T).Name);
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.UpdateFailed(typeof(T).Name, ex.Message));
        }
    }
    
    public IChangeToken GetReloadToken<T>() where T : class, IConfigurationDefinition<T>
    {
        var sectionName = GetSectionName<T>();
        return _changeTokenFactory.CreateChangeToken(sectionName);
    }
    
    private void InvalidateCache(Type configType)
    {
        _configurationCache.TryRemove(configType, out _);
        _logger.LogDebug("Invalidated configuration cache for {ConfigType}", configType.Name);
    }
}
```

## 🌐 Configuration Scopes & Hierarchy

### **Hierarchical Configuration Management**

Manage configuration at different levels with inheritance:

```csharp
public enum ConfigurationScope
{
    Global = 0,        // System-wide settings
    Application = 1,   // Application-level settings
    Environment = 2,   // Environment-specific settings (dev/staging/prod)
    Tenant = 3,        // Tenant-specific settings (multi-tenant scenarios)
    User = 4,          // User-specific settings
    Session = 5        // Session-specific settings
}

[ServiceRegistration(ServiceLifetime.Scoped)]
public class ScopedConfigurationService : IScopedConfigurationService
{
    private readonly IConfigurationHierarchy _hierarchy;
    private readonly ILogger<ScopedConfigurationService> _logger;
    
    public async Task<IEnvelope<T, ConfigurationCode, ConfigurationError>> GetScopedConfigurationAsync<T>(
        ConfigurationScope requestedScope = ConfigurationScope.Application,
        string? scopeId = null)
        where T : class, IConfigurationDefinition<T>, new()
    {
        try
        {
            var mergedConfig = new T();
            var appliedScopes = new List<ConfigurationScope>();
            
            // Apply configuration from each scope in hierarchy order (least to most specific)
            var scopes = GetApplicableScopes(requestedScope);
            
            foreach (var scope in scopes)
            {
                var scopeConfig = await GetConfigurationForScope<T>(scope, scopeId);
                if (scopeConfig.IsSuccess)
                {
                    mergedConfig = MergeConfigurations(mergedConfig, scopeConfig.Value);
                    appliedScopes.Add(scope);
                }
            }
            
            // Validate the merged configuration
            var validationResult = mergedConfig.Validate();
            if (!validationResult.IsValid)
            {
                return Envelope.ValidationError<T, ConfigurationCode, ConfigurationError>(
                    ConfigurationError.ValidationFailed(typeof(T).Name, validationResult.Errors));
            }
            
            _logger.LogDebug("Merged configuration for {ConfigType} from scopes: {Scopes}", 
                typeof(T).Name, string.Join(", ", appliedScopes));
                
            return Envelope.Success(ConfigurationCode.Retrieved, mergedConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scoped configuration for {ConfigType}", typeof(T).Name);
            return Envelope.Error<T, ConfigurationCode, ConfigurationError>(
                ConfigurationError.LoadFailed(typeof(T).Name, ex.Message));
        }
    }
    
    private IEnumerable<ConfigurationScope> GetApplicableScopes(ConfigurationScope maxScope)
    {
        var scopes = new List<ConfigurationScope>();
        
        // Always start with global
        scopes.Add(ConfigurationScope.Global);
        
        // Add intermediate scopes up to the requested scope
        if (maxScope >= ConfigurationScope.Application)
            scopes.Add(ConfigurationScope.Application);
            
        if (maxScope >= ConfigurationScope.Environment)
            scopes.Add(ConfigurationScope.Environment);
            
        if (maxScope >= ConfigurationScope.Tenant)
            scopes.Add(ConfigurationScope.Tenant);
            
        if (maxScope >= ConfigurationScope.User)
            scopes.Add(ConfigurationScope.User);
            
        if (maxScope >= ConfigurationScope.Session)
            scopes.Add(ConfigurationScope.Session);
            
        return scopes;
    }
    
    private T MergeConfigurations<T>(T baseConfig, T overrideConfig) where T : class
    {
        // Use reflection to merge non-null properties from override to base
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var merged = (T)Activator.CreateInstance(typeof(T), baseConfig)!;
        
        foreach (var property in properties.Where(p => p.CanWrite))
        {
            var overrideValue = property.GetValue(overrideConfig);
            if (overrideValue != null && !IsDefaultValue(overrideValue, property.PropertyType))
            {
                property.SetValue(merged, overrideValue);
            }
        }
        
        return merged;
    }
}

// Usage example for multi-tenant configuration
[ApiController]
public class TenantConfigurationController : ControllerBase
{
    private readonly IScopedConfigurationService _configService;
    
    [HttpGet("database-config")]
    public async Task<IActionResult> GetDatabaseConfig()
    {
        var tenantId = GetCurrentTenantId();
        
        var config = await _configService.GetScopedConfigurationAsync<DatabaseConfiguration>(
            ConfigurationScope.Tenant, tenantId);
            
        return config.ToActionResult();
    }
    
    [HttpPut("database-config")]
    public async Task<IActionResult> UpdateDatabaseConfig([FromBody] DatabaseConfiguration config)
    {
        var tenantId = GetCurrentTenantId();
        
        var result = await _configService.UpdateScopedConfigurationAsync(
            config, ConfigurationScope.Tenant, tenantId);
            
        return result.ToActionResult();
    }
}
```

### **Dynamic Configuration Updates**

Real-time configuration changes without application restart:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class DynamicConfigurationService : IDynamicConfigurationService
{
    private readonly IConfigurationRoot _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventPublisher _eventPublisher;
    private readonly ConcurrentDictionary<string, IDisposable> _changeTokenRegistrations;
    
    public DynamicConfigurationService(
        IConfigurationRoot configuration,
        IServiceProvider serviceProvider,
        IEventPublisher eventPublisher)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _eventPublisher = eventPublisher;
        _changeTokenRegistrations = new ConcurrentDictionary<string, IDisposable>();
        
        SetupChangeMonitoring();
    }
    
    private void SetupChangeMonitoring()
    {
        // Monitor configuration file changes
        var changeToken = _configuration.GetReloadToken();
        var registration = changeToken.RegisterChangeCallback(OnConfigurationChanged, null);
        _changeTokenRegistrations.TryAdd("root", registration);
    }
    
    private async void OnConfigurationChanged(object? state)
    {
        try
        {
            _logger.LogInformation("Configuration change detected, reloading...");
            
            // Reload configuration
            _configuration.Reload();
            
            // Notify all configuration-dependent services
            await NotifyConfigurationChangeAsync();
            
            // Re-register for next change
            SetupChangeMonitoring();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling configuration change");
        }
    }
    
    private async Task NotifyConfigurationChangeAsync()
    {
        var changeEvent = new ConfigurationReloadedEvent
        {
            Timestamp = DateTime.UtcNow,
            Source = "FileWatcher",
            AffectedSections = GetChangedSections()
        };
        
        await _eventPublisher.PublishAsync(changeEvent);
    }
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> ReloadConfigurationAsync()
    {
        try
        {
            _configuration.Reload();
            
            // Validate all registered configurations
            var validationTasks = GetRegisteredConfigurationTypes()
                .Select(ValidateConfigurationType);
                
            var validationResults = await Task.WhenAll(validationTasks);
            var failures = validationResults.Where(r => !r.IsValid).ToList();
            
            if (failures.Any())
            {
                var errorMessages = failures.SelectMany(f => f.Errors).ToList();
                return Envelope.ValidationError<ConfigurationCode, ConfigurationError>(
                    ConfigurationError.ReloadValidationFailed(errorMessages));
            }
            
            await NotifyConfigurationChangeAsync();
            
            return Envelope.Success(ConfigurationCode.Reloaded);
        }
        catch (Exception ex)
        {
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.ReloadFailed(ex.Message));
        }
    }
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> UpdateConfigurationSectionAsync(
        string sectionName,
        object configuration,
        bool validateOnly = false)
    {
        try
        {
            // Validate configuration
            var validationResult = await ValidateConfiguration(configuration);
            if (!validationResult.IsValid)
            {
                return Envelope.ValidationError<ConfigurationCode, ConfigurationError>(
                    ConfigurationError.ValidationFailed(sectionName, validationResult.Errors));
            }
            
            if (validateOnly)
            {
                return Envelope.Success(ConfigurationCode.ValidationPassed);
            }
            
            // Apply configuration update
            await ApplyConfigurationUpdate(sectionName, configuration);
            
            // Publish change event
            var changeEvent = new ConfigurationSectionUpdatedEvent
            {
                SectionName = sectionName,
                UpdatedBy = GetCurrentUser(),
                Timestamp = DateTime.UtcNow,
                ValidationResult = validationResult
            };
            
            await _eventPublisher.PublishAsync(changeEvent);
            
            return Envelope.Success(ConfigurationCode.Updated);
        }
        catch (Exception ex)
        {
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.UpdateFailed(sectionName, ex.Message));
        }
    }
}

// Configuration change event handlers
[EventHandlerRegistration(ServiceLifetime.Scoped)]
public class DatabaseConfigurationChangeHandler : 
    IEventHandler<ConfigurationSectionUpdatedEvent, ConfigurationCode, ConfigurationError>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseConfigurationChangeHandler> _logger;
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> HandleAsync(
        ConfigurationSectionUpdatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (@event.SectionName != "Database")
            return Envelope.Success(ConfigurationCode.Ignored);
            
        try
        {
            // Update database connection settings
            await _connectionFactory.ReloadConnectionsAsync();
            
            _logger.LogInformation("Database configuration updated successfully");
            
            return Envelope.Success(ConfigurationCode.Applied);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying database configuration change");
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.ApplyFailed("Database", ex.Message));
        }
    }
}
```

## 🔒 Configuration Security & Compliance

### **Encrypted Configuration Values**

Automatic encryption/decryption of sensitive configuration data:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class SecureConfigurationService : ISecureConfigurationService
{
    private readonly IConfigurationEncryption _encryption;
    private readonly IConfigurationAuditing _auditing;
    private readonly ILogger<SecureConfigurationService> _logger;
    
    public async Task<IEnvelope<T, ConfigurationCode, ConfigurationError>> GetSecureConfigurationAsync<T>()
        where T : class, IConfigurationDefinition<T>, new()
    {
        try
        {
            var config = await LoadRawConfigurationAsync<T>();
            if (!config.IsSuccess)
                return config;
                
            // Decrypt sensitive properties
            var decryptedConfig = await DecryptSensitiveProperties(config.Value);
            
            // Audit configuration access
            await _auditing.LogConfigurationAccessAsync(typeof(T).Name, GetCurrentUser());
            
            return Envelope.Success(ConfigurationCode.Retrieved, decryptedConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading secure configuration for {ConfigType}", typeof(T).Name);
            return Envelope.Error<T, ConfigurationCode, ConfigurationError>(
                ConfigurationError.DecryptionFailed(typeof(T).Name, ex.Message));
        }
    }
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> SetSecureConfigurationAsync<T>(
        T configuration)
        where T : class, IConfigurationDefinition<T>
    {
        try
        {
            // Encrypt sensitive properties before storing
            var encryptedConfig = await EncryptSensitiveProperties(configuration);
            
            // Store encrypted configuration
            var result = await StoreConfigurationAsync(encryptedConfig);
            if (!result.IsSuccess)
                return result;
                
            // Audit configuration change
            await _auditing.LogConfigurationChangeAsync(
                typeof(T).Name, 
                GetCurrentUser(), 
                "Updated",
                GetSensitivePropertyNames<T>());
                
            return Envelope.Success(ConfigurationCode.Updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing secure configuration for {ConfigType}", typeof(T).Name);
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.EncryptionFailed(typeof(T).Name, ex.Message));
        }
    }
    
    private async Task<T> DecryptSensitiveProperties<T>(T configuration) where T : class
    {
        var sensitiveProperties = GetSensitiveProperties<T>();
        
        foreach (var property in sensitiveProperties)
        {
            var encryptedValue = property.GetValue(configuration) as string;
            if (!string.IsNullOrEmpty(encryptedValue))
            {
                var decryptedValue = await _encryption.DecryptAsync(encryptedValue);
                property.SetValue(configuration, decryptedValue);
            }
        }
        
        return configuration;
    }
    
    private IEnumerable<PropertyInfo> GetSensitiveProperties<T>()
    {
        return typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null);
    }
}

// Mark sensitive configuration properties
public record PaymentConfiguration : IConfigurationDefinition<PaymentConfiguration>
{
    public string PaymentGatewayUrl { get; init; } = string.Empty;
    
    [SensitiveData] // This property will be encrypted
    public string ApiKey { get; init; } = string.Empty;
    
    [SensitiveData] // This property will be encrypted
    public string WebhookSecret { get; init; } = string.Empty;
    
    public int TimeoutSeconds { get; init; } = 30;
    public bool EnableRetries { get; init; } = true;
}

[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute : Attribute
{
    public string? EncryptionAlgorithm { get; set; }
    public bool RequireKeyRotation { get; set; } = false;
    public int KeyRotationDays { get; set; } = 90;
}
```

### **Configuration Compliance & Auditing**

Track configuration changes for compliance requirements:

```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class ConfigurationAuditingService : IConfigurationAuditing
{
    private readonly IAuditRepository _auditRepository;
    private readonly IComplianceService _complianceService;
    private readonly ILogger<ConfigurationAuditingService> _logger;
    
    public async Task<IEnvelope<ConfigurationCode, ConfigurationError>> LogConfigurationChangeAsync(
        string configurationType,
        string changedBy,
        string changeType,
        IEnumerable<string> affectedProperties)
    {
        try
        {
            var auditEntry = new ConfigurationAuditEntry
            {
                Id = Guid.NewGuid().ToString(),
                ConfigurationType = configurationType,
                ChangeType = changeType,
                ChangedBy = changedBy,
                Timestamp = DateTime.UtcNow,
                AffectedProperties = affectedProperties.ToList(),
                SessionId = GetCurrentSessionId(),
                IPAddress = GetCurrentIPAddress(),
                UserAgent = GetCurrentUserAgent(),
                ComplianceFlags = await GetComplianceFlags(configurationType)
            };
            
            // Store audit entry
            await _auditRepository.CreateAsync(auditEntry);
            
            // Check compliance requirements
            await CheckComplianceRequirements(auditEntry);
            
            _logger.LogInformation(
                "Configuration change audited: {ConfigType} {ChangeType} by {User}",
                configurationType, changeType, changedBy);
                
            return Envelope.Success(ConfigurationCode.Audited);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auditing configuration change");
            return Envelope.Error<ConfigurationCode, ConfigurationError>(
                ConfigurationError.AuditFailed(ex.Message));
        }
    }
    
    private async Task CheckComplianceRequirements(ConfigurationAuditEntry auditEntry)
    {
        var complianceChecks = await _complianceService.GetRequiredChecksAsync(auditEntry.ConfigurationType);
        
        foreach (var check in complianceChecks)
        {
            var result = await check.ValidateAsync(auditEntry);
            if (!result.IsCompliant)
            {
                await HandleComplianceViolation(auditEntry, check, result);
            }
        }
    }
    
    private async Task HandleComplianceViolation(
        ConfigurationAuditEntry auditEntry,
        IComplianceCheck check,
        ComplianceResult result)
    {
        var violation = new ComplianceViolation
        {
            AuditEntryId = auditEntry.Id,
            ComplianceRule = check.RuleName,
            ViolationType = result.ViolationType,
            Description = result.Description,
            Severity = result.Severity,
            RequiresImmediateAction = result.RequiresImmediateAction
        };
        
        await _complianceService.ReportViolationAsync(violation);
        
        if (result.RequiresImmediateAction)
        {
            await _notificationService.SendUrgentNotificationAsync(
                "Compliance Violation",
                $"Configuration change for {auditEntry.ConfigurationType} violates {check.RuleName}",
                GetComplianceOfficers());
        }
    }
}

public record ConfigurationAuditEntry
{
    public string Id { get; init; } = string.Empty;
    public string ConfigurationType { get; init; } = string.Empty;
    public string ChangeType { get; init; } = string.Empty;
    public string ChangedBy { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public List<string> AffectedProperties { get; init; } = new();
    public string? SessionId { get; init; }
    public string? IPAddress { get; init; }
    public string? UserAgent { get; init; }
    public List<string> ComplianceFlags { get; init; } = new();
    public Dictionary<string, object> AdditionalMetadata { get; init; } = new();
}
```

---

**Zentient's configuration management system provides sophisticated configuration handling with type safety, hierarchical scoping, dynamic updates, security, and comprehensive compliance tracking for enterprise applications.**
