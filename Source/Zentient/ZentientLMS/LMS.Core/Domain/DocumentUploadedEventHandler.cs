using LMS.Core.Domain.Events.Contracts;
using LMS.Core.Entities;
using LMS.Core.Handlers.Events;
using LMS.Core.Infrastructure.Throttling;

namespace LMS.Core.Domain
{
    public class DocumentUploadedEventHandler : IDomainEventHandler<DocumentUploadedEvent>
    {
        private readonly ThrottleManager _throttleManager;

        public DocumentUploadedEventHandler(ThrottleManager throttleManager)
        {
            _throttleManager = throttleManager;
        }

        public async Task Handle(DocumentUploadedEvent domainEvent)
        {
            if (!_throttleManager.TryProcessEvent())
            {
                // Throttling limit reached, log or handle accordingly
                return;
            }

            // Process the event
            await ProcessDocumentUploadAsync(domainEvent.Document);
        }

        private async Task ProcessDocumentUploadAsync(Document document)
        {
            await Task.CompletedTask;
        }
    }
}
