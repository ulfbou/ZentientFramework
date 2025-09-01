using System;
using System.Threading.Tasks;
using Zentient.DependencyInjection.Registrations;

namespace Zentient.DependencyInjection.Examples
{
    // ========================================================================================
    // EXAMPLE 1: Basic Service Registration
    // ========================================================================================
    
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    /// <summary>
    /// Basic service registration with explicit contract specification.
    /// This is the most common pattern for service registration.
    /// </summary>
    [Service(ServiceLifetime.Scoped, Description = "Primary email service for the application")]
    [ProvidesContract(typeof(IEmailService), Primary = true)]
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Implementation here
            return Task.CompletedTask;
        }
    }

    // ========================================================================================
    // EXAMPLE 2: Conditional Service Registration
    // ========================================================================================
    
    /// <summary>
    /// Service that only gets registered in development environments.
    /// Demonstrates environmental conditional registration.
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    [ProvidesContract(typeof(IEmailService))]
    [ServiceCondition("Development", "Testing")]
    public class MockEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"MOCK EMAIL: To={to}, Subject={subject}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Service with configuration-based conditional registration.
    /// Only registered when specific feature flags are enabled.
    /// </summary>
    [Service(ServiceLifetime.Scoped)]
    [ProvidesContract(typeof(IEmailService))]
    [ServiceCondition("Features:AdvancedEmail", "true")]
    [ServiceCondition("Email:Provider", "SendGrid", "Mailgun")]
    public class AdvancedEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Advanced email implementation
            return Task.CompletedTask;
        }
    }

    // ========================================================================================
    // EXAMPLE 3: Service with Multiple Contracts
    // ========================================================================================
    
    public interface INotificationService
    {
        Task NotifyAsync(string message);
    }

    public interface ILoggingService  
    {
        void Log(string message);
    }

    /// <summary>
    /// Service implementing multiple contracts with different priorities.
    /// Demonstrates how one service can provide multiple interfaces.
    /// </summary>
    [Service(ServiceLifetime.Singleton, 
        Priority = 10,
        Tags = new[] { "communication", "logging" },
        Category = "Infrastructure",
        Metadata = new[] { "Source=Internal", "Version=2.0" })]
    [ProvidesContract(typeof(INotificationService), Primary = true)]
    [ProvidesContract(typeof(ILoggingService))]
    public class CommunicationService : INotificationService, ILoggingService
    {
        public Task NotifyAsync(string message)
        {
            // Notification implementation
            return Task.CompletedTask;
        }

        public void Log(string message)
        {
            // Logging implementation
        }
    }

    // ========================================================================================
    // EXAMPLE 4: Service Decorator Pattern
    // ========================================================================================
    
    /// <summary>
    /// Decorator that adds logging to email operations.
    /// Demonstrates the decorator pattern with automatic ordering.
    /// </summary>
    [ServiceDecorator(typeof(IEmailService), Order = 1, Description = "Adds logging to email operations")]
    public class LoggingEmailDecorator : IEmailService
    {
        private readonly IEmailService _inner;
        private readonly ILoggingService _logger;

        public LoggingEmailDecorator(IEmailService inner, ILoggingService logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.Log($"Sending email to {to} with subject: {subject}");
            try
            {
                await _inner.SendEmailAsync(to, subject, body);
                _logger.Log($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to send email to {to}: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Decorator that adds retry logic to email operations.
    /// This decorator runs before the logging decorator (Order = 0 < 1).
    /// </summary>
    [ServiceDecorator(typeof(IEmailService), Order = 0, Description = "Adds retry logic to email operations")]
    public class RetryEmailDecorator : IEmailService
    {
        private readonly IEmailService _inner;
        private const int MaxRetries = 3;

        public RetryEmailDecorator(IEmailService inner)
        {
            _inner = inner;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    await _inner.SendEmailAsync(to, subject, body);
                    return; // Success
                }
                catch (Exception) when (attempt < MaxRetries)
                {
                    // Retry logic - wait before next attempt
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }
        }
    }

    // ========================================================================================
    // EXAMPLE 5: Service Factory Pattern
    // ========================================================================================
    
    public interface IReportGenerator
    {
        Task<string> GenerateReportAsync(string reportType);
    }

    /// <summary>
    /// Factory for creating report generators based on runtime conditions.
    /// Demonstrates the factory pattern for dynamic service creation.
    /// </summary>
    [ServiceFactory(typeof(IReportGenerator), 
        Description = "Creates report generators based on configuration",
        Category = "Factories")]
    public class ReportGeneratorFactory : IServiceFactory<IReportGenerator>
    {
        public IReportGenerator Create(IServiceProvider serviceProvider)
        {
            // Factory logic to determine which implementation to create
            // Could be based on configuration, environment, or other factors
            return new PdfReportGenerator();
        }
    }

    public class PdfReportGenerator : IReportGenerator
    {
        public Task<string> GenerateReportAsync(string reportType)
        {
            return Task.FromResult($"PDF Report: {reportType}");
        }
    }

    // ========================================================================================
    // EXAMPLE 6: Keyed Services
    // ========================================================================================
    
    /// <summary>
    /// Service registered with a specific key for targeted resolution.
    /// Useful when you need multiple implementations of the same contract.
    /// </summary>
    [Service(ServiceLifetime.Scoped)]
    [ProvidesContract(typeof(IEmailService), ServiceKey = "production")]
    public class ProductionEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Production email implementation
            return Task.CompletedTask;
        }
    }

    [Service(ServiceLifetime.Scoped)]
    [ProvidesContract(typeof(IEmailService), ServiceKey = "development")]
    public class DevelopmentEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Development email implementation
            Console.WriteLine($"DEV EMAIL: {to} - {subject}");
            return Task.CompletedTask;
        }
    }

    // ========================================================================================
    // EXAMPLE 7: Excluded Services
    // ========================================================================================
    
    /// <summary>
    /// Abstract base class that should not be registered automatically.
    /// The ExcludeFromRegistration attribute prevents automatic scanning from registering this.
    /// </summary>
    [ExcludeFromRegistration("Abstract base class - not for direct registration")]
    public abstract class BaseEmailService : IEmailService
    {
        public abstract Task SendEmailAsync(string to, string subject, string body);
        
        protected virtual void ValidateEmailAddress(string email)
        {
            // Common validation logic
        }
    }

    /// <summary>
    /// Service that should be manually registered with custom configuration.
    /// </summary>
    [ExcludeFromRegistration("Requires manual registration with custom configuration")]
    public class CustomConfigEmailService : BaseEmailService
    {
        public override Task SendEmailAsync(string to, string subject, string body)
        {
            ValidateEmailAddress(to);
            // Custom implementation
            return Task.CompletedTask;
        }
    }
}
