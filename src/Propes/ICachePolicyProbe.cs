// <copyright file="ICachePolicyProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Zentient.Abstractions.Caching;
using Zentient.Abstractions.Caching.Definitions;
using Zentient.Abstractions.DependencyInjection;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.Abstractions.DependencyInjection.Results;
using Zentient.Abstractions.DependencyInjection.Validation;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Results;
using Zentient.Testing.Diagnostics.Abstractions;

namespace Zentient.Testing.Probes
{
    public interface ICacheDiagnostics
    {
        IReadOnlyCollection<CachePolicyUsage> Usages { get; }
    }

    public record CachePolicyUsage(CachePolicy Policy, object Key, object? Value);

    public interface IRetryDiagnostics
    {
        int GetRetryCount();
        IReadOnlyCollection<Exception> GetExceptions();
    }

    public interface IDIConfigurationProbe : IProbe, IDIConfigurationDiagnostics
    {
    }

    public interface IDIConfigurationDiagnostics
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
        // Runs a full graph validation based on a provided options object.
        Task<IResult> ValidateAsync(ServiceValidationOptions? options = null, CancellationToken cancellationToken = default);

        // Analyzes the graph structure and returns a dependency graph object.
        Task<IServiceDependencyGraph> AnalyzeAsync(Type? rootServiceType = null, CancellationToken cancellationToken = default);

        // Quick checks for common pitfalls.
        bool HasCircularDependencies();
        bool HasLifetimeMismatches();
        bool HasUnusedServices();
    }

    public interface IDIGraphDiagnostics
    {
        Task<IResult> ValidateAsync(ServiceValidationOptions? options = null, CancellationToken cancellationToken = default);
        Task<IServiceDependencyGraph> AnalyzeAsync(Type? rootServiceType = null, CancellationToken cancellationToken = default);
        bool HasCircularDependencies();
        bool HasLifetimeMismatches();
        bool HasUnusedServices();
    }

    public interface IResolutionProbe
    {
        // A log of every service resolution attempt.
        IReadOnlyCollection<IServiceResolutionResult<object>> ResolutionLog { get; }

        // Simulates an execution context for scoped services.
        IDisposable UseScope(string? scopeId = null, IMetadata? metadata = null);

        // Retrieves a specific resolution result from the log.
        IServiceResolutionResult<TService>? GetLastResolutionFor<TService>();
        IEnumerable<IServiceResolutionResult<TService>> GetAllResolutionsFor<TService>();
    }

    public interface IResolutionDiagnostics
    {
        IReadOnlyCollection<IServiceResolutionResult<object>> ResolutionLog { get; }
        IDisposable UseScope(string? scopeId = null, IMetadata? metadata = null);
        IServiceResolutionResult<TService>? GetLastResolutionFor<TService>();
        IEnumerable<IServiceResolutionResult<TService>> GetAllResolutionsFor<TService>();
    }
    public interface ICachePolicyProbe
    {
        IReadOnlyCollection<CachePolicyUsage> Usages { get; }
    }

    public interface IRetryProbe
    {
        int GetRetryCount();
        IReadOnlyCollection<Exception> GetExceptions();
    }
}
