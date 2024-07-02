using LMS.Core.Search;

namespace LMS.Core.Services
{
    public interface IBetterRequestService<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TReadDto> CreateAsync<TCreateDto, TReadDto>(TCreateDto createDto, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<IEnumerable<TReadDto>> CreateBulkAsync<TCreateDto, TReadDto>(IEnumerable<TCreateDto> createDtos, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<bool> DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        Task<bool> DeleteBulkAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> Items, int TotalCount, int TotalPages)> GetAsync(PaginationParameters parameters, CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> items, int totalCount, object totalPages)> SearchAsync<T>(SearchParameters searchParameters);
        Task<TReadDto> UpdateAsync<TKey, TUpdateDto, TReadDto>(TKey id, TUpdateDto updateDto, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<IEnumerable<TReadDto>> UpdateBulkAsync<TKey, TUpdateDto, TReadDto>(IEnumerable<KeyValuePair<TKey, TUpdateDto>> updates, CancellationToken cancellationToken = default) where TReadDto : class;
    }
    public interface IBetterRequestService<TEntity, TKey> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TReadDto> CreateAsync<TCreateDto, TReadDto>(TCreateDto createDto, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<IEnumerable<TReadDto>> CreateBulkAsync<TCreateDto, TReadDto>(IEnumerable<TCreateDto> createDtos, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<bool> DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        Task<bool> DeleteBulkAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> Items, int TotalCount, int TotalPages)> GetAsync(PaginationParameters parameters, CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> items, int totalCount, object totalPages)> SearchAsync<T>(SearchParameters searchParameters);
        Task<TReadDto> UpdateAsync<TKey, TUpdateDto, TReadDto>(TKey id, TUpdateDto updateDto, CancellationToken cancellationToken = default) where TReadDto : class;
        Task<IEnumerable<TReadDto>> UpdateBulkAsync<TKey, TUpdateDto, TReadDto>(IEnumerable<KeyValuePair<TKey, TUpdateDto>> updates, CancellationToken cancellationToken = default) where TReadDto : class;
        Task UpdateBulkAsync<T1, T2>(IEnumerable<object> updateDtos);
    }
}