using LMS.Core.Domain.Events.Contracts;
using LMS.Core.Services.Communication;

namespace LMS.Core.Handlers.Events
{
    public class WelcomeEmailEventHandler : IDomainEventHandler<NewUserRegisteredEvent>
    {
        private readonly EmailService _emailService;

        public WelcomeEmailEventHandler(EmailService emailService)
        {
            _emailService = emailService;
        }

        public Task Handle(NewUserRegisteredEvent domainEvent)
        {
            // Directly use the _emailService instance
            _emailService.SendWelcomeEmail(domainEvent.Email);
            return Task.CompletedTask;
        }
    }
}
