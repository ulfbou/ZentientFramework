
namespace Zentient.GenericRepository
{
    [Serializable]
    internal class QueryExecutionException : Exception
    {
        public QueryExecutionException()
        {
        }

        public QueryExecutionException(string? message) : base(message)
        {
        }

        public QueryExecutionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}