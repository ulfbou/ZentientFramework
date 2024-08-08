using LMS.Core.Domain.Events.Contracts;

namespace LMS.Core.Domain
{
    public class ErrorOccurredEvent : IDomainEvent
    {
        public string ErrorMessage { get; }
        public Exception Exception { get; }

        public ErrorOccurredEvent(string errorMessage, Exception exception)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }
    }
}
