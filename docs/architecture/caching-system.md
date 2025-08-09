# 🗄️ Caching System Architecture

## Overview

Zentient.Abstractions provides a sophisticated caching system that goes beyond simple key-value storage. It offers typed cache definitions, automatic invalidation strategies, distributed caching support, and comprehensive observability. The system integrates seamlessly with the envelope pattern for consistent error handling and provides rich metadata for cache operations.

## 🎯 Core Concepts

### **Cache Definitions as Architecture**

Caches are not just storage mechanisms - they are architectural components with rich metadata and behaviors:

```csharp
[CacheDefinition("UserCache", Version = "2.1.0")]
public record UserCacheDefinition : ICacheDefinition<UserDto>
{
    public string Id => "UserCache.v2.1";
    public string Name => "User Data Cache";
    public string Description => "Caches user profile information with automatic invalidation";
    
    public Type ValueType => typeof(UserDto);
    public TimeSpan DefaultTtl => TimeSpan.FromMinutes(30);
    public TimeSpan MaxTtl => TimeSpan.FromHours(24);
    public CacheEvictionPolicy EvictionPolicy => CacheEvictionPolicy.LRU;
    public int MaxItems => 10000;
    public bool AllowNullValues => false;
    public bool CompressValues => true;
    public bool EncryptSensitiveData => true;
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "UserManagement",
        ["DataClassification"] = "Personal",
        ["ComplianceRequirement"] = "GDPR",
        ["InvalidationStrategy"] = "EventDriven",
        ["ReplicationStrategy"] = "MultiRegion",
        ["BackupFrequency"] = "Hourly",
        ["MonitoringLevel"] = "Detailed",
        ["PerformanceTarget"] = "< 5ms",
        ["HitRateTarget"] = "> 95%",
        ["Dependencies"] = new[] { "UserService", "ProfileService" },
        ["InvalidationEvents"] = new[]
        {
            "UserUpdated",
            "UserDeleted", 
            "ProfileChanged",
            "PermissionsChanged"
        }
    };
    
    // Key generation strategy
    public string GenerateKey(params object[] keyParts)
    {
        if (keyParts.Length == 1 && keyParts[0] is string userId)
        {
            return $"User:{userId}";
        }
        
        if (keyParts.Length == 2 && keyParts[0] is string userId2 && keyParts[1] is string version)
        {
            return $"User:{userId2}:v{version}";
        }
        
        throw new ArgumentException("Invalid key parts for UserCache");
    }
    
    // Validation rules for cached values
    public ValidationResult ValidateValue(UserDto value)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(value.Id))
            errors.Add("User ID is required");
            
        if (string.IsNullOrEmpty(value.Email))
            errors.Add("Email is required");
            
        if (value.CreatedAt > DateTime.UtcNow)
            errors.Add("Created date cannot be in the future");
            
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
    
    // Serialization configuration
    public CacheSerializationOptions SerializationOptions => new()
    {
        UseCompression = true,
        CompressionThreshold = 1024, // Compress values larger than 1KB
        EncryptionKey = "UserDataEncryptionKey",
        SerializationFormat = SerializationFormat.MessagePack
    };
}
```

### **Strongly-Typed Cache Operations**

Type-safe cache operations with automatic validation and error handling:

```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserCacheService : ICacheService<UserDto>
{
    private readonly ICache<UserDto> _cache;
    private readonly ILogger<UserCacheService> _logger;
    private readonly IMetrics _metrics;
    
    public UserCacheService(
        ICache<UserDto> cache,
        ILogger<UserCacheService> logger,
        IMetrics metrics)
    {
        _cache = cache;
        _logger = logger;
        _metrics = metrics;
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> SetUserAsync(
        string userId, 
        UserDto user,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = Activity.StartActivity("SetUser");
        activity?.SetTag("user.id", userId);
        
        try
        {
            var key = _cache.Definition.GenerateKey(userId);
            var effectiveTtl = ttl ?? _cache.Definition.DefaultTtl;
            
            // Validate the user data before caching
            var validation = _cache.Definition.ValidateValue(user);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Invalid user data for caching: {Errors}", 
                    string.Join(", ", validation.Errors));
                return Envelope.ValidationError<CacheCode, CacheError>(
                    CacheError.InvalidValue(validation.Errors));
            }
            
            // Set with metadata for tracking
            var cacheItem = new CacheItem<UserDto>
            {
                Key = key,
                Value = user,
                Ttl = effectiveTtl,
                Metadata = new MetadataCollection
                {
                    ["UserId"] = userId,
                    ["CachedAt"] = DateTime.UtcNow,
                    ["Source"] = "UserService",
                    ["Version"] = user.Version?.ToString() ?? "1.0"
                }
            };
            
            var result = await _cache.SetAsync(cacheItem, cancellationToken);
            
            if (result.IsSuccess)
            {
                _metrics.IncrementCounter("cache.set.success", new()
                {
                    ["cache"] = "UserCache",
                    ["operation"] = "SetUser"
                });
                
                _logger.LogDebug("Cached user {UserId} with TTL {TTL}", userId, effectiveTtl);
            }
            else
            {
                _metrics.IncrementCounter("cache.set.failure", new()
                {
                    ["cache"] = "UserCache",
                    ["operation"] = "SetUser",
                    ["error"] = result.Error?.Code
                });
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching user {UserId}", userId);
            return Envelope.Error<CacheCode, CacheError>(
                CacheError.OperationFailed($"Failed to cache user: {ex.Message}"));
        }
    }
    
    public async Task<IEnvelope<UserDto, CacheCode, CacheError>> GetUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        using var activity = Activity.StartActivity("GetUser");
        activity?.SetTag("user.id", userId);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var key = _cache.Definition.GenerateKey(userId);
            var result = await _cache.GetAsync(key, cancellationToken);
            
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;
            
            if (result.IsSuccess)
            {
                _metrics.IncrementCounter("cache.hit", new()
                {
                    ["cache"] = "UserCache",
                    ["operation"] = "GetUser"
                });
                
                _metrics.RecordDuration("cache.get.duration", duration, new()
                {
                    ["cache"] = "UserCache",
                    ["result"] = "hit"
                });
                
                _logger.LogDebug("Cache hit for user {UserId} in {Duration}ms", userId, duration);
                
                return Envelope.Success(CacheCode.Hit, result.Value);
            }
            else if (result.Error?.Code == "NotFound")
            {
                _metrics.IncrementCounter("cache.miss", new()
                {
                    ["cache"] = "UserCache",
                    ["operation"] = "GetUser"
                });
                
                _metrics.RecordDuration("cache.get.duration", duration, new()
                {
                    ["cache"] = "UserCache",
                    ["result"] = "miss"
                });
                
                _logger.LogDebug("Cache miss for user {UserId} in {Duration}ms", userId, duration);
                
                return Envelope.NotFound<UserDto, CacheCode, CacheError>(
                    CacheError.KeyNotFound(key));
            }
            else
            {
                _metrics.IncrementCounter("cache.error", new()
                {
                    ["cache"] = "UserCache",
                    ["operation"] = "GetUser",
                    ["error"] = result.Error?.Code
                });
                
                return result.ConvertError<UserDto, CacheCode, CacheError>();
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error retrieving user {UserId} from cache", userId);
            
            return Envelope.Error<UserDto, CacheCode, CacheError>(
                CacheError.OperationFailed($"Failed to retrieve user: {ex.Message}"));
        }
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> InvalidateUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var key = _cache.Definition.GenerateKey(userId);
        var result = await _cache.RemoveAsync(key, cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Invalidated cache for user {UserId}", userId);
            
            // Trigger invalidation event for dependent caches
            await PublishInvalidationEventAsync(userId, cancellationToken);
        }
        
        return result;
    }
    
    private async Task PublishInvalidationEventAsync(string userId, CancellationToken cancellationToken)
    {
        var @event = new CacheInvalidatedEvent
        {
            CacheType = "UserCache",
            Key = userId,
            Reason = "UserInvalidation",
            Timestamp = DateTime.UtcNow
        };
        
        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}
```

## 🚀 Cache Patterns & Strategies

### **Cache-Aside Pattern**

Implement cache-aside pattern with automatic fallback:

```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ICacheService<UserDto> _cache;
    private readonly IMapper _mapper;
    
    public async Task<IEnvelope<UserDto, UserCode, UserError>> GetUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Try cache first
        var cacheResult = await _cache.GetUserAsync(userId, cancellationToken);
        if (cacheResult.IsSuccess)
        {
            return cacheResult.ConvertTo<UserDto, UserCode, UserError>(UserCode.Retrieved);
        }
        
        // Cache miss - get from repository
        var repositoryResult = await _repository.GetByIdAsync(userId);
        if (!repositoryResult.IsSuccess)
        {
            return repositoryResult.ConvertError<UserDto, UserCode, UserError>();
        }
        
        // Map to DTO
        var userDto = _mapper.Map<UserDto>(repositoryResult.Value);
        
        // Cache for future requests (fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await _cache.SetUserAsync(userId, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache user {UserId}", userId);
            }
        }, cancellationToken);
        
        return Envelope.Success(UserCode.Retrieved, userDto);
    }
    
    public async Task<IEnvelope<UserCode, UserError>> UpdateUserAsync(
        string userId,
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Update in repository
        var updateResult = await _repository.UpdateAsync(userId, command);
        if (!updateResult.IsSuccess)
        {
            return updateResult.ConvertError<UserCode, UserError>();
        }
        
        // Invalidate cache immediately
        await _cache.InvalidateUserAsync(userId, cancellationToken);
        
        // Optionally warm cache with new data
        var updatedUser = _mapper.Map<UserDto>(updateResult.Value);
        await _cache.SetUserAsync(userId, updatedUser, cancellationToken);
        
        return Envelope.Success(UserCode.Updated);
    }
}
```

### **Write-Through Caching**

Automatic cache updates on data changes:

```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class WriteThroughUserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ICacheService<UserDto> _cache;
    
    public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            // Create in repository
            var createResult = await _repository.CreateAsync(command);
            if (!createResult.IsSuccess)
            {
                return createResult.ConvertError<UserCode, UserError>();
            }
            
            var user = createResult.Value;
            var userDto = _mapper.Map<UserDto>(user);
            
            // Write to cache immediately
            var cacheResult = await _cache.SetUserAsync(user.Id, userDto, cancellationToken);
            if (!cacheResult.IsSuccess)
            {
                // Cache failure shouldn't fail the operation, but log it
                _logger.LogWarning("Failed to cache newly created user {UserId}: {Error}", 
                    user.Id, cacheResult.Error?.Message);
            }
            
            await transaction.CommitAsync();
            
            return Envelope.Success(UserCode.Created);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating user with write-through cache");
            return Envelope.Error<UserCode, UserError>(
                UserError.CreationFailed($"Failed to create user: {ex.Message}"));
        }
    }
}
```

### **Write-Behind (Write-Back) Caching**

Asynchronous cache-to-storage synchronization:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class WriteBehindCacheService : IWriteBehindCacheService<UserDto>
{
    private readonly ICacheService<UserDto> _cache;
    private readonly IUserRepository _repository;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ConcurrentDictionary<string, WriteBehindEntry<UserDto>> _pendingWrites;
    
    public async Task<IEnvelope<CacheCode, CacheError>> SetAsync(
        string key,
        UserDto value,
        CancellationToken cancellationToken = default)
    {
        // Update cache immediately
        var cacheResult = await _cache.SetUserAsync(key, value, cancellationToken);
        if (!cacheResult.IsSuccess)
        {
            return cacheResult;
        }
        
        // Queue for asynchronous write to storage
        var entry = new WriteBehindEntry<UserDto>
        {
            Key = key,
            Value = value,
            QueuedAt = DateTime.UtcNow,
            RetryCount = 0
        };
        
        _pendingWrites.AddOrUpdate(key, entry, (k, existing) => entry);
        
        // Schedule background write
        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            await WriteToStorageAsync(entry, token);
        });
        
        return cacheResult;
    }
    
    private async Task WriteToStorageAsync(
        WriteBehindEntry<UserDto> entry,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Batch delay
            
            var userId = ExtractUserIdFromKey(entry.Key);
            var updateCommand = _mapper.Map<UpdateUserCommand>(entry.Value);
            
            var result = await _repository.UpdateAsync(userId, updateCommand);
            
            if (result.IsSuccess)
            {
                _pendingWrites.TryRemove(entry.Key, out _);
                _logger.LogDebug("Successfully wrote user {UserId} to storage", userId);
            }
            else
            {
                await HandleWriteFailureAsync(entry, result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing cache entry {Key} to storage", entry.Key);
            await HandleWriteFailureAsync(entry, ex);
        }
    }
    
    private async Task HandleWriteFailureAsync(WriteBehindEntry<UserDto> entry, object error)
    {
        entry.RetryCount++;
        entry.LastError = error;
        
        if (entry.RetryCount <= 3)
        {
            // Exponential backoff retry
            var delay = TimeSpan.FromSeconds(Math.Pow(2, entry.RetryCount));
            
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(delay, token);
                await WriteToStorageAsync(entry, token);
            });
        }
        else
        {
            // Move to dead letter queue for manual intervention
            await _deadLetterQueue.EnqueueAsync(entry);
            _pendingWrites.TryRemove(entry.Key, out _);
            
            _logger.LogError("Failed to write cache entry {Key} after {RetryCount} attempts", 
                entry.Key, entry.RetryCount);
        }
    }
}
```

## 🔄 Cache Invalidation Strategies

### **Event-Driven Invalidation**

Automatic cache invalidation based on domain events:

```csharp
[EventHandlerRegistration(ServiceLifetime.Scoped)]
public class CacheInvalidationEventHandler : 
    IEventHandler<UserUpdatedEvent, CacheCode, CacheError>,
    IEventHandler<UserDeletedEvent, CacheCode, CacheError>,
    IEventHandler<PermissionsChangedEvent, CacheCode, CacheError>
{
    private readonly ICacheInvalidationService _invalidationService;
    
    public async Task<IEnvelope<CacheCode, CacheError>> HandleAsync(
        UserUpdatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        var invalidationStrategy = new InvalidationStrategy
        {
            PrimaryKeys = new[] { $"User:{@event.UserId}" },
            PatternKeys = new[] { $"User:{@event.UserId}:*" },
            DependentCaches = new[] { "UserProfile", "UserPermissions", "UserPreferences" },
            CascadeInvalidation = true,
            InvalidationReason = "UserUpdated"
        };
        
        return await _invalidationService.InvalidateAsync(invalidationStrategy, cancellationToken);
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> HandleAsync(
        UserDeletedEvent @event,
        CancellationToken cancellationToken = default)
    {
        var invalidationStrategy = new InvalidationStrategy
        {
            PrimaryKeys = new[] { $"User:{@event.UserId}" },
            PatternKeys = new[] 
            { 
                $"User:{@event.UserId}:*",
                $"UserSessions:{@event.UserId}:*",
                $"UserActivity:{@event.UserId}:*"
            },
            DependentCaches = new[] { "UserProfile", "UserPermissions", "UserPreferences", "UserSessions" },
            CascadeInvalidation = true,
            InvalidationReason = "UserDeleted",
            PurgeRelatedData = true
        };
        
        return await _invalidationService.InvalidateAsync(invalidationStrategy, cancellationToken);
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> HandleAsync(
        PermissionsChangedEvent @event,
        CancellationToken cancellationToken = default)
    {
        // More targeted invalidation for permission changes
        var invalidationStrategy = new InvalidationStrategy
        {
            PatternKeys = new[] 
            { 
                $"UserPermissions:{@event.UserId}:*",
                $"RolePermissions:{@event.RoleId}:*",
                $"AccessControl:*:{@event.UserId}:*"
            },
            DependentCaches = new[] { "UserPermissions", "RoleCache", "AccessControl" },
            CascadeInvalidation = false, // Avoid over-invalidation
            InvalidationReason = "PermissionsChanged"
        };
        
        return await _invalidationService.InvalidateAsync(invalidationStrategy, cancellationToken);
    }
}

[ServiceRegistration(ServiceLifetime.Scoped)]
public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<CacheInvalidationService> _logger;
    private readonly IMetrics _metrics;
    
    public async Task<IEnvelope<CacheCode, CacheError>> InvalidateAsync(
        InvalidationStrategy strategy,
        CancellationToken cancellationToken = default)
    {
        var invalidatedKeys = new List<string>();
        var errors = new List<CacheError>();
        
        try
        {
            // Invalidate primary keys
            foreach (var key in strategy.PrimaryKeys)
            {
                var result = await _cacheProvider.RemoveAsync(key, cancellationToken);
                if (result.IsSuccess)
                {
                    invalidatedKeys.Add(key);
                }
                else
                {
                    errors.Add(result.Error);
                }
            }
            
            // Invalidate pattern-based keys
            foreach (var pattern in strategy.PatternKeys)
            {
                var keys = await _cacheProvider.GetKeysAsync(pattern, cancellationToken);
                foreach (var key in keys)
                {
                    var result = await _cacheProvider.RemoveAsync(key, cancellationToken);
                    if (result.IsSuccess)
                    {
                        invalidatedKeys.Add(key);
                    }
                    else
                    {
                        errors.Add(result.Error);
                    }
                }
            }
            
            // Cascade invalidation to dependent caches
            if (strategy.CascadeInvalidation)
            {
                await CascadeInvalidationAsync(strategy.DependentCaches, strategy.InvalidationReason, cancellationToken);
            }
            
            // Record metrics
            _metrics.IncrementCounter("cache.invalidation", new()
            {
                ["reason"] = strategy.InvalidationReason,
                ["keys_count"] = invalidatedKeys.Count.ToString(),
                ["errors_count"] = errors.Count.ToString()
            });
            
            _logger.LogInformation("Invalidated {KeyCount} cache keys for reason: {Reason}", 
                invalidatedKeys.Count, strategy.InvalidationReason);
            
            return errors.Any() 
                ? Envelope.PartialSuccess<CacheCode, CacheError>(CacheCode.PartiallyInvalidated, errors.First())
                : Envelope.Success(CacheCode.Invalidated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache invalidation for reason: {Reason}", strategy.InvalidationReason);
            return Envelope.Error<CacheCode, CacheError>(
                CacheError.InvalidationFailed($"Cache invalidation failed: {ex.Message}"));
        }
    }
}
```

### **Time-Based Invalidation**

Automatic expiration and refresh strategies:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class TimeBasedCacheManager : ITimeBasedCacheManager
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<TimeBasedCacheManager> _logger;
    private readonly Timer _cleanupTimer;
    
    public TimeBasedCacheManager(ICacheProvider cacheProvider, ILogger<TimeBasedCacheManager> logger)
    {
        _cacheProvider = cacheProvider;
        _logger = logger;
        
        // Schedule cleanup every 5 minutes
        _cleanupTimer = new Timer(PerformCleanup, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> SetWithSlidingExpirationAsync<T>(
        string key,
        T value,
        TimeSpan slidingExpiration,
        CancellationToken cancellationToken = default)
    {
        var cacheItem = new CacheItem<T>
        {
            Key = key,
            Value = value,
            SlidingExpiration = slidingExpiration,
            LastAccessed = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _cacheProvider.SetAsync(cacheItem, cancellationToken);
    }
    
    public async Task<IEnvelope<T, CacheCode, CacheError>> GetWithSlidingExpirationAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var result = await _cacheProvider.GetAsync<T>(key, cancellationToken);
        
        if (result.IsSuccess)
        {
            // Update last accessed time for sliding expiration
            await UpdateLastAccessedAsync(key, cancellationToken);
        }
        
        return result;
    }
    
    private async void PerformCleanup(object? state)
    {
        try
        {
            var expiredKeys = await _cacheProvider.GetExpiredKeysAsync();
            var cleanupTasks = expiredKeys.Select(key => _cacheProvider.RemoveAsync(key));
            
            await Task.WhenAll(cleanupTasks);
            
            if (expiredKeys.Any())
            {
                _logger.LogInformation("Cleaned up {ExpiredCount} expired cache entries", expiredKeys.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache cleanup");
        }
    }
}
```

## 🌐 Distributed Caching

### **Multi-Level Cache Hierarchy**

Implement L1 (local) and L2 (distributed) cache levels:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class MultiLevelCacheService : ICacheService<UserDto>
{
    private readonly IMemoryCache _l1Cache;     // Local memory cache
    private readonly IRedisCache _l2Cache;      // Distributed Redis cache
    private readonly ILogger<MultiLevelCacheService> _logger;
    
    public async Task<IEnvelope<UserDto, CacheCode, CacheError>> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        // Try L1 cache first (fastest)
        var l1Result = await _l1Cache.GetAsync<UserDto>(key);
        if (l1Result.IsSuccess)
        {
            _metrics.IncrementCounter("cache.l1.hit");
            return Envelope.Success(CacheCode.L1Hit, l1Result.Value);
        }
        
        // Try L2 cache (distributed)
        var l2Result = await _l2Cache.GetAsync<UserDto>(key, cancellationToken);
        if (l2Result.IsSuccess)
        {
            _metrics.IncrementCounter("cache.l2.hit");
            
            // Promote to L1 cache for faster access
            await _l1Cache.SetAsync(key, l2Result.Value, TimeSpan.FromMinutes(5));
            
            return Envelope.Success(CacheCode.L2Hit, l2Result.Value);
        }
        
        _metrics.IncrementCounter("cache.miss");
        return Envelope.NotFound<UserDto, CacheCode, CacheError>(
            CacheError.KeyNotFound(key));
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> SetAsync(
        string key,
        UserDto value,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveTtl = ttl ?? TimeSpan.FromMinutes(30);
        var errors = new List<CacheError>();
        
        // Set in L2 cache first (persistent)
        var l2Result = await _l2Cache.SetAsync(key, value, effectiveTtl, cancellationToken);
        if (!l2Result.IsSuccess)
        {
            errors.Add(l2Result.Error);
        }
        
        // Set in L1 cache (shorter TTL for memory efficiency)
        var l1Ttl = TimeSpan.FromMinutes(Math.Min(effectiveTtl.TotalMinutes, 10));
        var l1Result = await _l1Cache.SetAsync(key, value, l1Ttl);
        if (!l1Result.IsSuccess)
        {
            errors.Add(l1Result.Error);
        }
        
        if (errors.Count == 2)
        {
            return Envelope.Error<CacheCode, CacheError>(errors.First());
        }
        else if (errors.Count == 1)
        {
            return Envelope.PartialSuccess<CacheCode, CacheError>(CacheCode.PartiallySet, errors.First());
        }
        
        return Envelope.Success(CacheCode.Set);
    }
    
    public async Task<IEnvelope<CacheCode, CacheError>> InvalidateAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var l1Task = _l1Cache.RemoveAsync(key);
        var l2Task = _l2Cache.RemoveAsync(key, cancellationToken);
        
        var results = await Task.WhenAll(l1Task, l2Task);
        
        var errors = results.Where(r => !r.IsSuccess).Select(r => r.Error).ToList();
        
        if (errors.Any())
        {
            return Envelope.Error<CacheCode, CacheError>(errors.First());
        }
        
        return Envelope.Success(CacheCode.Invalidated);
    }
}
```

### **Cache Synchronization**

Keep distributed cache instances synchronized:

```csharp
[ServiceRegistration(ServiceLifetime.Singleton)]
public class CacheSynchronizationService : ICacheSynchronizationService
{
    private readonly IMessageBus _messageBus;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<CacheSynchronizationService> _logger;
    
    public async Task<IEnvelope<CacheCode, CacheError>> InvalidateAcrossClusterAsync(
        string key,
        string reason,
        CancellationToken cancellationToken = default)
    {
        // Invalidate locally first
        await _cacheProvider.RemoveAsync(key, cancellationToken);
        
        // Broadcast invalidation message to other nodes
        var invalidationMessage = new CacheInvalidationMessage
        {
            Key = key,
            Reason = reason,
            NodeId = Environment.MachineName,
            Timestamp = DateTime.UtcNow
        };
        
        await _messageBus.PublishAsync("cache.invalidation", invalidationMessage, cancellationToken);
        
        return Envelope.Success(CacheCode.InvalidatedAcrossCluster);
    }
    
    [MessageHandler("cache.invalidation")]
    public async Task HandleCacheInvalidationAsync(CacheInvalidationMessage message)
    {
        // Don't process our own invalidation messages
        if (message.NodeId == Environment.MachineName)
            return;
            
        _logger.LogDebug("Received cache invalidation for key {Key} from node {NodeId}", 
            message.Key, message.NodeId);
            
        await _cacheProvider.RemoveAsync(message.Key);
        
        _metrics.IncrementCounter("cache.remote_invalidation", new()
        {
            ["key"] = message.Key,
            ["reason"] = message.Reason,
            ["source_node"] = message.NodeId
        });
    }
}
```

---

**Zentient's caching system provides sophisticated caching capabilities with type safety, automatic invalidation, distributed support, and comprehensive observability for building high-performance applications.**
