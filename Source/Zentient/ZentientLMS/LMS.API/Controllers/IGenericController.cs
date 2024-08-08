namespace LMS.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Zentient.Repository;

    public interface IController<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        // GET: {EntityEndpoint}
        [HttpGet]
        public Task<ActionResult<IEnumerable<TDto>>> Get<TDto, TDtoKey>(string? title, bool? inclusion = true, int pageIndex = 1, int pageSize = 10)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct;

        // GET: {EntityEndpoint}/{id}
        [HttpGet("id")]
        public Task<ActionResult<TDto>> Get<TDto, TDtoKey>(TKey id, bool? inclusion)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct;

        // Post: {EntityEndpoint}
        [HttpPost]
        [Authorize]
        public Task<ActionResult<TDto>> Create<TDto, TDtoKey>(TDto entityDto)
            where TDto : class
            where TDtoKey : struct;

        // Put: {EntityEndpoint}/{id}
        [HttpPut("{id}")]
        [Authorize]
        public Task<IActionResult> Update<TDto, TDtoKey>(TKey id, TDto entityDto)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct;

        // Delete: {EntityEndpoint}/{id}
        [HttpDelete("{id}")][Authorize] public Task<IActionResult> Delete(TKey id);
    }
}