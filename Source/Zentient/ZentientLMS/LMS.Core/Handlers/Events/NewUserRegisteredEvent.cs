using LMS.Core.Domain.Events.Contracts;

namespace LMS.Core.Handlers.Events
{
    public class NewUserRegisteredEvent : IDomainEvent
    {
        public string UserId { get; }
        public string Email { get; }

        public NewUserRegisteredEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
