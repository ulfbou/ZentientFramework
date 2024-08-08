using LMS.Core.Infrastructure.Caching;
using LMS.Core.Search;
using Microsoft.AspNetCore.Mvc;

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
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TEntity, TKey, TCreateDto, TReadDto, TUpdateDto> : ControllerBase
        where TEntity : class
        where TReadDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        private readonly IBetterControllerService<TEntity, TKey> _requestService;
        private readonly IBetterResponseService<TEntity, TKey> _responseService;
        private readonly IBetterSearchService _searchService;

        public GenericController(IBetterSearchService searchService, IBetterControllerService<TEntity, TKey> requestService, IBetterResponseService<TEntity, TKey> responseService)
        {
            _searchService = searchService;
            _requestService = requestService;
            _responseService = responseService;
        }

        // GET: api/[controller]
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationParameters parameters)
        {
            var (items, totalCount, totalPages) = await _requestService.GetAsync(parameters);
            var response = _responseService.ReadResponse<TReadDto>(items, parameters, totalCount);
            return Ok(response);
        }

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TCreateDto createDto)
        {
            var readDto = await _requestService.CreateAsync<TCreateDto, TReadDto>(createDto);
            var response = _responseService.CreateResponse<TReadDto>(readDto);
            return Ok(response);
        }

        // PUT: api/[controller]/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(TKey id, [FromBody] TUpdateDto updateDto)
        {
            var readDto = await _requestService.UpdateAsync<TKey, TUpdateDto, TReadDto>(id, updateDto);
            var response = _responseService.UpdateResponse<TReadDto>(readDto);
            return Ok(response);
        }

        // DELETE: api/[controller]/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TKey id)
        {
            var success = await _requestService.DeleteAsync(id);
            return _responseService.DeleteResponse(success);
        }

        // POST: api/[controller]/bulk
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulk([FromBody] IEnumerable<TCreateDto> createDtos)
        {
            var readDtos = await _requestService.CreateBulkAsync<TCreateDto, TReadDto>(createDtos);
            return Ok(_responseService.BulkResponse<TReadDto>(readDtos));
        }

        // DELETE: api/[controller]/bulk
        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteBulk([FromBody] IEnumerable<TKey> ids)
        {
            var success = await _requestService.DeleteBulkAsync(ids);
            return Ok(_responseService.DeleteResponse(success));
        }

        // Additional endpoints for bulk update and other operations can be added here following the same pattern.
        // PUT: api/[controller]/bulk
        [HttpPut("bulk")]
        public async Task<IActionResult> UpdateBulk([FromBody] IEnumerable<TUpdateDto> updateDtos)
        {
            var readDtos = await _requestService.UpdateBulkAsync<TUpdateDto, TReadDto>(updateDtos);
            return Ok(_responseService.BulkResponse<TReadDto>(readDtos));
        }

        // Additional custom operations can be implemented here.
        // For example, a search operation that uses complex filtering criteria.

        // GET: api/[controller]/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] SearchParameters parameters)
        {
            if (!_searchService.IsValidSearchableColumn(parameters.SearchableColumnName))
            {
                return BadRequest("Invalid searchable column name.");
            }

            return Ok(await _searchService.SearchAsync(parameters));
        }

        [HttpPost("update-cache-settings")]
        public IActionResult UpdateCacheSettings([FromBody] CacheSettings settings)
        {
            _searchService.UpdateCacheSettings(settings);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync2([FromQuery] SearchParameters searchParameters)
        {
            var (items, totalCount, totalPages) = await _requestService.SearchAsync<TReadDto>(searchParameters);
            var response = _responseService.ReadResponse<TReadDto>(items, new PaginationParameters
            {
                PageNumber = searchParameters.PageNumber,
                PageSize = searchParameters.PageSize
            }, totalCount);
            return Ok(response);
        }

    }
}
