// <copyright file="ConfigurationBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// </copyright>

using System.Dynamic;
using System.Linq.Expressions;

using Zentient.Abstractions.Caching;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration;
using Zentient.Abstractions.DependencyInjection;
using Zentient.Abstractions.DependencyInjection.Results;
using Zentient.Abstractions.DependencyInjection.Validation;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Results;
using Zentient.Testing.Abstractions;

namespace Zentient.Testing.Propes
{
    public record CachePolicyUsage(CachePolicy Policy, object Key, object? Value);

    public interface IRetryProbe
    {
        int GetRetryCount();
        IReadOnlyCollection<Exception> GetExceptions();
    }
    public interface IDIConfigurationProbe
    {
        bool HasService<TService>();
        bool HasService(Type serviceContract);
        bool HasLifetime<TService>(ServiceLifetime lifetime);
        bool HasMetadata<TService>(string key, object? expectedValue);
        bool IsDecorated<TService, TDecorator>();
        bool IsIntercepted<TService, TInterceptor>();
        IServiceRegistry Registry { get; }
    }

    public interface IDIGraphProbe
    {
        Task<IResult> ValidateAsync(ServiceValidationOptions? options = null, CancellationToken cancellationToken = default);
        Task<IServiceDependencyGraph> AnalyzeAsync(Type? rootServiceType = null, CancellationToken cancellationToken = default);
        bool HasCircularDependencies();
        bool HasLifetimeMismatches();
        bool HasUnusedServices();
    }

    public interface IResolutionProbe
    {
        IReadOnlyCollection<IServiceResolutionResult<object>> ResolutionLog { get; }
        IDisposable UseScope(string? scopeId = null, IMetadata? metadata = null);
        IServiceResolutionResult<TService>? GetLastResolutionFor<TService>();
        IEnumerable<IServiceResolutionResult<TService>> GetAllResolutionsFor<TService>();
    }
}
