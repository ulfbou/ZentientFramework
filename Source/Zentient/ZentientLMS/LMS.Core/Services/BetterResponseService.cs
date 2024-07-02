using AutoMapper;
using LMS.Core.Search;
using Microsoft.AspNetCore.Mvc;

// how do we handle null values during the entity to DTO mapping process?
// how do we handle the case where the entity is not found?
// how do we handle the case where the entity is found but the DTO is not?
namespace LMS.Core.Services
{
    public class BetterResponseService<TEntity, TKey> : IBetterResponseService<TEntity, TKey> where TEntity : class
    {
        private readonly IMapper _mapper;

        public BetterResponseService(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ActionResult<TDto> CreateResponse<TDto>(TEntity entity) where TDto : class
        {
            if (entity == null)
            {
                return new NotFoundResult();
            }

            var dto = _mapper.Map<TDto>(entity);
            return new CreatedAtActionResult("GetById", "ControllerName", new { id = GetEntityId(entity) }, dto);
        }

        public ActionResult<IEnumerable<TDto>> ReadResponse<TDto>(IEnumerable<TEntity> entities, PaginationParameters parameters, int totalItems) where TDto : class
        {
            if (entities == null || !entities.Any())
            {
                return new NotFoundResult();
            }

            var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
            var totalPages = (int)Math.Ceiling((double)totalItems / parameters.PageSize);
            var metadata = new
            {
                parameters.PageNumber,
                parameters.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return new OkObjectResult(new { Data = dtos, Meta = metadata });
        }

        public ActionResult<TDto> UpdateResponse<TDto>(TEntity entity) where TDto : class
        {
            if (entity == null)
            {
                return new NotFoundResult();
            }

            var dto = _mapper.Map<TDto>(entity);
            return new OkObjectResult(dto);
        }

        // The DeleteResponse method remains unchanged as it does not involve entity-to-DTO conversion.

        public ActionResult<IEnumerable<TReadDto>> BulkResponse<TReadDto>(IEnumerable<TEntity> entities) where TReadDto : class
        {
            if (entities == null || !entities.Any())
            {
                return new NotFoundResult();
            }

            var dtos = _mapper.Map<IEnumerable<TReadDto>>(entities);
            return new OkObjectResult(dtos);
        }

        private TKey GetEntityId(TEntity entity)
        {
            // Implement logic to extract the ID from the entity
            throw new NotImplementedException();
        }

        public IActionResult DeleteResponse(object success)
        {
            throw new NotImplementedException();
        }
    }
}
