using LMS.Core.Search;

// how do we handle authorization for different user roles in this generic controller?
// how do we handle the response for different user roles in this generic controller?
// how do we customize the error handlng in the BetterResponseService to provide more detailed feedback for failed operations?
// how do we handle the search operation in the generic controller?
// how do we implement the 'search parameters' for the custom search operation in the generic controller?
// how do we handle the case where the search operation returns no results?
// how do we set up global error handling in an asp.net core api?

// how can we validate the SearchableColumnName to ensure it exists on the TEntity type?
// how can we validate the SearchOperator to ensure it is a valid operator?
// how can we implement a dynamic search method and adapt it to support full-text search functions like ef.functions.freetext?


namespace LMS.Core.Services
{
    public interface IBetterControllerService<TEntity, TKey> where TEntity : class
    {
        Task<TEntity> CreateAsync<T1, T2>(object createDto);
        Task<IEnumerable<TEntity>> CreateBulkAsync<T1, T2>(IEnumerable<object> createDtos);
        Task<T> DeleteAsync<T>(T id);
        Task<T> DeleteBulkAsync<T>(IEnumerable<T> ids);
        Task<(IEnumerable<TEntity> items, int totalCount, object totalPages)> GetAsync(PaginationParameters parameters);
        Task<(IEnumerable<TEntity> items, int totalCount, object totalPages)> SearchAsync<T>(SearchParameters searchParameters);
        Task<TEntity> UpdateAsync<T1, T2, T3>(T1 id, object updateDto);
        Task<IEnumerable<TEntity>> UpdateBulkAsync<T1, T2>(IEnumerable<object> updateDtos);
    }
}
