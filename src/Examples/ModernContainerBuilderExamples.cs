using System;
using System.Reflection;
using System.Threading.Tasks;
using Zentient.Abstractions.DependencyInjection.Builders;
using Zentient.DependencyInjection.Registrations;
using ServiceLifetime = Zentient.DependencyInjection.Registrations.ServiceLifetime;

namespace Zentient.DependencyInjection.Examples
{
    /// <summary>
    /// Comprehensive examples demonstrating the IModernContainerBuilder capabilities.
    /// Shows the most DX-friendly, versatile, and sophisticated usage patterns.
    /// </summary>
    public static class ModernContainerBuilderExamples
    {
        /// <summary>
        /// Example 1: Basic attribute-driven registration with automatic discovery.
        /// This is the simplest and most common pattern.
        /// </summary>
        public static async Task<IServiceProvider> BasicAttributeRegistrationExample()
        {
            var builder = CreateBuilder();

            // Register services decorated with [Service] and [ProvidesContract] attributes
            builder.Register<EmailService>()
                   .Register<PaymentService>()
                   .Register<ConcreteUserService>();

            // Validate and build
            var validationReport = await builder.ValidateAsync();
            if (!validationReport.IsValid)
            {
                throw new InvalidOperationException($"Configuration errors: {validationReport.Summary}");
            }

            return await builder.BuildAsync();
        }

        /// <summary>
        /// Example 2: Fluent registration with full type safety and rich configuration.
        /// Demonstrates the power of fluent configuration.
        /// </summary>
        public static async Task<IServiceProvider> FluentRegistrationExample()
        {
            var builder = CreateBuilder();

            builder
                // Register with explicit types and rich configuration
                .Register<IEmailService, EmailService>(config => config
                    .AsScoped()
                    .WithMetadata("Provider", "SendGrid")
                    .WithMetadata("ApiVersion", "v3")
                    .WithTags("communication", "external", "production")
                    .AsPrimary()
                    .WithKey("production"))

                // Register with factory for complex initialization
                .RegisterFactory<IPaymentGateway>(provider => 
                {
                    var config = (IConfiguration)provider.GetService(typeof(IConfiguration))!;
                    var apiKey = config.GetConnectionString("PaymentGateway");
                    return new StripePaymentGateway(apiKey);
                }, config => config
                    .AsSingleton()
                    .WithMetadata("Gateway", "Stripe")
                    .WithTags("payment", "external"))

                // Register singleton instances
                .RegisterInstance<ILogger>(new ConsoleLogger(), config => config
                    .WithMetadata("Type", "Console")
                    .WithTags("logging", "development")
                    .AsPrimary());

            return await builder.BuildAsync();
        }

        /// <summary>
        /// Example 3: Assembly scanning with advanced filtering.
        /// Shows how to automatically register services from assemblies with sophisticated control.
        /// </summary>
        public static async Task<IServiceProvider> AssemblyScanningExample()
        {
            var builder = CreateBuilder();

            builder
                // Scan current assembly with default settings
                .ScanCurrentAssembly()

                // Scan specific assemblies
                .ScanAssemblies(
                    Assembly.GetExecutingAssembly(),
                    typeof(SomeService).Assembly)

                // Advanced scanning with filtering
                .ScanAssemblies(scan => scan
                    .FromAssemblyContaining<Program>()
                    .FromAssemblyContaining<EmailService>()
                    .IncludeNamespaces(
                        "MyApp.Services",
                        "MyApp.Infrastructure", 
                        "MyApp.Repositories")
                    .ExcludeTypes(type => 
                        type.IsAbstract || 
                        type.Name.EndsWith("Test") ||
                        type.Name.Contains("Mock"))
                    .WithDefaultLifetime(Zentient.Abstractions.DependencyInjection.Builders.ServiceLifetime.Scoped));

            return await builder.BuildAsync();
        }

        /// <summary>
        /// Example 4: Conditional registration based on environment and configuration.
        /// Demonstrates environment-aware and configuration-driven service registration.
        /// </summary>
        public static async Task<IServiceProvider> ConditionalRegistrationExample()
        {
            var builder = CreateBuilder();

            builder
                // Register based on runtime conditions
                .RegisterWhen(
                    () => Environment.GetEnvironmentVariable("USE_MOCK_SERVICES") == "true",
                    conditional => conditional
                        .Register<IEmailService, MockEmailService>()
                        .Register<IPaymentGateway, MockPaymentGateway>()
                        .Register<ISmsService, MockSmsService>())

                // Register for specific environments
                .RegisterForEnvironments(
                    new[] { "Development", "Testing" },
                    env => env
                        .Register<IEmailService, DebugEmailService>()
                        .Register<IFileStorage, LocalFileStorage>())

                .RegisterForEnvironments(
                    new[] { "Production", "Staging" },
                    env => env
                        .Register<IEmailService, SendGridEmailService>()
                        .Register<IFileStorage, AzureBlobStorage>())

                // Register with configuration-based conditions
                .RegisterWhen(
                    () => GetConfigValue("Features:AdvancedLogging") == "true",
                    conditional => conditional
                        .Register<ILogger, AdvancedLogger>()
                        .Register<IMetricsCollector, DetailedMetricsCollector>());

            return await builder.BuildAsync();
        }

        /// <summary>
        /// Example 5: Module-based organization for complex applications.
        /// Shows how to organize services into reusable modules.
        /// </summary>
        public static async Task<IServiceProvider> ModuleBasedExample()
        {
            var builder = CreateBuilder();

            builder
                // Register individual modules
                .UseModule<EmailModule>()
                .UseModule<PaymentModule>()
                .UseModule<LoggingModule>()
                .UseModule<DatabaseModule>()

                // Register module instances with configuration
                .UseModule(new CustomSecurityModule(
                    jwtSecret: GetConfigValue("JWT:Secret"),
                    tokenExpiry: TimeSpan.FromHours(24)))

                // Mix modules with direct registrations
                .Register<IUserService, ConcreteUserService>(config => config
                    .AsScoped()
                    .WithTags("core", "user-management"));

            return await builder.BuildAsync();
        }

        /// <summary>
        /// Example 6: Comprehensive validation and diagnostics.
        /// Demonstrates the powerful validation and diagnostic capabilities.
        /// </summary>
        public static async Task<IServiceProvider> ValidationAndDiagnosticsExample()
        {
            var builder = CreateBuilder();

            // Configure services
            builder
                .ScanCurrentAssembly()
                .UseModule<EmailModule>()
                .RegisterFactory<IComplexService>(provider => new ComplexService());

            // Get diagnostics before building
            var diagnostics = builder.GetDiagnostics();
            Console.WriteLine($"Total registrations: {diagnostics.TotalRegistrations}");
            Console.WriteLine($"Modules: {string.Join(", ", diagnostics.RegisteredModules)}");
            
            foreach (var (lifetime, count) in diagnostics.LifetimeDistribution)
            {
                Console.WriteLine($"{lifetime}: {count} services");
            }

            // Validate configuration
            var validationReport = await builder.ValidateAsync();
            
            if (!validationReport.IsValid)
            {
                Console.WriteLine($"Validation failed with {validationReport.Errors.Count} errors:");
                foreach (var error in validationReport.Errors)
                {
                    Console.WriteLine($"  [{error.Severity}] {error.Message}");
                    if (error.ServiceType != null)
                        Console.WriteLine($"    Service: {error.ServiceType.Name}");
                    if (!string.IsNullOrEmpty(error.Details))
                        Console.WriteLine($"    Details: {error.Details}");
                }
                
                throw new InvalidOperationException("Container configuration is invalid");
            }

            Console.WriteLine($"Validation successful: {validationReport.Summary}");
            return await builder.BuildAsync(validateFirst: false); // Already validated
        }

        /// <summary>
        /// Example 7: Advanced configuration and customization.
        /// Shows how to customize container behavior and leverage advanced features.
        /// </summary>
        public static async Task<IServiceProvider> AdvancedConfigurationExample()
        {
            var builder = CreateBuilder();

            // Configure container behavior
            builder.AutoRegisterFrameworkServices = true;  // Register IServiceProvider, etc.
            builder.EagerValidation = true;                // Validate on each registration
            builder.AllowMultipleRegistrations = true;     // Allow multiple implementations

            builder
                // Register multiple implementations with keys
                .Register<IEmailService, SendGridEmailService>(config => config
                    .WithKey("sendgrid")
                    .AsScoped()
                    .WithMetadata("Provider", "SendGrid"))
                
                .Register<IEmailService, MailgunEmailService>(config => config
                    .WithKey("mailgun")
                    .AsScoped()
                    .WithMetadata("Provider", "Mailgun"))

                // Register primary implementation without key
                .Register<IEmailService, DefaultEmailService>(config => config
                    .AsPrimary()
                    .AsScoped())

                // Register with complex factory
                .RegisterFactory<IAdvancedService>(provider =>
                {
                    var emailService = (IEmailService)provider.GetService(typeof(IEmailService))!;
                    var logger = (ILogger)provider.GetService(typeof(ILogger))!;
                    var config = (IConfiguration)provider.GetService(typeof(IConfiguration))!;
                    
                    return new AdvancedService(emailService, logger, config);
                }, config => config
                    .AsSingleton()
                    .WithMetadata("Complexity", "High")
                    .WithTags("advanced", "composite"));

            // Check registered types before building
            Console.WriteLine($"Registered {builder.RegisteredTypes.Count} service types:");
            foreach (var type in builder.RegisteredTypes)
            {
                Console.WriteLine($"  - {type.Name}");
            }

            return await builder.BuildAsync();
        }

        // ================================================================================
        // HELPER METHODS AND EXAMPLE CLASSES
        // ================================================================================

        private static IModernContainerBuilder CreateBuilder()
        {
            // In a real implementation, this would create the actual builder
            // For this example, it's a placeholder
            throw new NotImplementedException("Actual implementation would create IModernContainerBuilder");
        }

        private static string GetConfigValue(string key)
        {
            // Placeholder for configuration access
            return Environment.GetEnvironmentVariable(key) ?? "false";
        }

        // Example service interfaces and implementations for demonstration
        public interface IEmailService { Task SendEmailAsync(string to, string subject, string body); }
        public interface IPaymentGateway { Task<PaymentResult> ProcessPaymentAsync(Payment payment); }
        public interface ILogger { void Log(string message); }
        public interface IUserService { Task<User> GetUserAsync(int id); }
        public interface ISmsService { Task SendSmsAsync(string to, string message); }
        public interface IFileStorage { Task<string> SaveFileAsync(byte[] content, string fileName); }
        public interface IMetricsCollector { void RecordMetric(string name, double value); }
        public interface IComplexService { void DoComplexWork(); }
        public interface IAdvancedService { Task<string> ProcessAsync(string input); }
        public interface IConfiguration { string GetConnectionString(string name); }

        // Example implementations
        [Service(ServiceLifetime.Scoped)]
        [ProvidesContract(typeof(IEmailService))]
        public class EmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
        }

        public class SendGridEmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
        }

        public class MailgunEmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
        }

        public class DefaultEmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
        }

        public class MockEmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body)
            {
                Console.WriteLine($"MOCK EMAIL: {to} - {subject}");
                return Task.CompletedTask;
            }
        }

        // Additional example classes...
        public class PaymentService { }
        public class ConcreteUserService : IUserService 
        { 
            public Task<User> GetUserAsync(int id) => Task.FromResult(new User()); 
        }
        public class SomeService { }
        public class Program { }
        public class StripePaymentGateway : IPaymentGateway
        {
            public StripePaymentGateway(string apiKey) { }
            public Task<PaymentResult> ProcessPaymentAsync(Payment payment) => Task.FromResult(new PaymentResult());
        }

        public class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine(message);
        }

        // Supporting types
        public class PaymentResult { }
        public class Payment { }
        public class User { }

        // Example modules
        public class EmailModule : IContainerModule
        {
            public string Name => "Email Services";
            public string Version => "1.0.0";

            public void Configure(IModernContainerBuilder builder)
            {
                builder.Register<IEmailService, EmailService>(config => config.AsScoped());
            }
        }

        public class PaymentModule : IContainerModule
        {
            public string Name => "Payment Services";
            public string Version => "1.0.0";

            public void Configure(IModernContainerBuilder builder)
            {
                builder.RegisterFactory<IPaymentGateway>(provider => 
                    new StripePaymentGateway("test-key"));
            }
        }

        public class LoggingModule : IContainerModule
        {
            public string Name => "Logging Services";
            public string Version => "1.0.0";

            public void Configure(IModernContainerBuilder builder)
            {
                builder.RegisterInstance<ILogger>(new ConsoleLogger());
            }
        }

        public class DatabaseModule : IContainerModule
        {
            public string Name => "Database Services";
            public string Version => "1.0.0";

            public void Configure(IModernContainerBuilder builder)
            {
                // Database-related registrations would go here
            }
        }

        public class CustomSecurityModule : IContainerModule
        {
            private readonly string _jwtSecret;
            private readonly TimeSpan _tokenExpiry;

            public CustomSecurityModule(string jwtSecret, TimeSpan tokenExpiry)
            {
                _jwtSecret = jwtSecret;
                _tokenExpiry = tokenExpiry;
            }

            public string Name => "Custom Security";
            public string Version => "1.0.0";

            public void Configure(IModernContainerBuilder builder)
            {
                // Security-related registrations using the configuration
            }
        }

        // Additional service implementations for examples
        public class DebugEmailService : IEmailService
        {
            public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
        }

        public class MockPaymentGateway : IPaymentGateway
        {
            public Task<PaymentResult> ProcessPaymentAsync(Payment payment) => Task.FromResult(new PaymentResult());
        }

        public class MockSmsService : ISmsService
        {
            public Task SendSmsAsync(string to, string message) => Task.CompletedTask;
        }

        public class LocalFileStorage : IFileStorage
        {
            public Task<string> SaveFileAsync(byte[] content, string fileName) => Task.FromResult($"local://{fileName}");
        }

        public class AzureBlobStorage : IFileStorage
        {
            public Task<string> SaveFileAsync(byte[] content, string fileName) => Task.FromResult($"azure://{fileName}");
        }

        public class AdvancedLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine($"[ADVANCED] {message}");
        }

        public class DetailedMetricsCollector : IMetricsCollector
        {
            public void RecordMetric(string name, double value) => Console.WriteLine($"Metric {name}: {value}");
        }

        public class ComplexService : IComplexService
        {
            public void DoComplexWork() { }
        }

        public class AdvancedService : IAdvancedService
        {
            public AdvancedService(IEmailService emailService, ILogger logger, IConfiguration config) { }
            public Task<string> ProcessAsync(string input) => Task.FromResult($"Processed: {input}");
        }
    }
}
