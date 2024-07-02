using LMS.Core.Search;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Core.Services
{
    public interface IBetterResponseService<TEntity, TKey> where TEntity : class
    {
        ActionResult<IEnumerable<TReadDto>> BulkResponse<TReadDto>(IEnumerable<TEntity> entities) where TReadDto : class;
        ActionResult<TDto> CreateResponse<TDto>(TEntity entity) where TDto : class;
        IActionResult DeleteResponse(object success);
        ActionResult<IEnumerable<TDto>> ReadResponse<TDto>(IEnumerable<TEntity> entities, PaginationParameters parameters, int totalItems) where TDto : class;
        ActionResult<TDto> UpdateResponse<TDto>(TEntity entity) where TDto : class;
    }
}