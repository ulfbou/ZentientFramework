namespace Zentient.Repository.QueryObjects
{
    public class QueryParameters<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> Query { get; }
        public string Id { get; set; }

        // Add more properties as needed

        public QueryParameters(IQueryable<TEntity> query, string id)
        {
            Query = query;
            Id = id;
        }
    }
}