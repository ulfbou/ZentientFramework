
namespace Zentient.GenericRepository.QueryObjects
{
    [Serializable]
    internal class InvalidBuildException : Exception
    {
        public InvalidBuildException()
        {
        }

        public InvalidBuildException(string? message) : base(message)
        {
        }

        public InvalidBuildException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}