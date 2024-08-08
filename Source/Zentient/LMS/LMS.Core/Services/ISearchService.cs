namespace LMS.Core.Services
{
    public interface ISearchService<TEntity>
    {
        Task<IEnumerable<TEntity>> SearchAsync(string query);
    }
}
