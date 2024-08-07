

using LMS.Core.Domain.Events.Contracts;

namespace LMS.Core.Domain.Events
{
    public class UserActionEvent : IDomainEvent
    {
        public string UserId { get; }
        public string Action { get; }
        public DateTime Timestamp { get; }

        public UserActionEvent(string userId, string action, DateTime timestamp)
        {
            UserId = userId;
            Action = action;
            Timestamp = timestamp;
        }
    }
}
