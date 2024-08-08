using LMS.Core.Domain;
using LMS.Core.Domain.Events.Contracts;
using Microsoft.Extensions.Logging;
using Zentient.Core.Helpers;

namespace LMS.Core.Handlers.Logging
{
    public class LoggingEventHandler : IDomainEventHandler<ErrorOccurredEvent>
    {
        protected ILogger<LoggingEventHandler> _logger = Factory.CreateLogger<LoggingEventHandler>();
        public Task Handle(ErrorOccurredEvent domainEvent)
        {
            // Log the error information
            _logger.LogError(domainEvent.ErrorMessage, domainEvent.Exception);
            return Task.CompletedTask;
        }
    }
}
