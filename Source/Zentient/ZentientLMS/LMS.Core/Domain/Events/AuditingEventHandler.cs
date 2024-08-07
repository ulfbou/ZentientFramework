using LMS.Core.Domain.Events.Contracts;

namespace LMS.Core.Domain.Events
{
    public class AuditingEventHandler : IDomainEventHandler<UserActionEvent>
    {
        public Task Handle(UserActionEvent domainEvent)
        {
            // Log the user action to an audit log
            AuditLog.Write(domainEvent.UserId, domainEvent.Action, domainEvent.Timestamp);
            return Task.CompletedTask;
        }
    }
}
