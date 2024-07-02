using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zentient.Repository;

namespace LMS.API.Controllers
{
    public partial class Controller<TEntity, TKey>
        : ControllerBase
        where TEntity : Zentient.Repository.BaseEntity<TKey>, IEntity<TKey>, new()
        where TKey : struct, IEquatable<TKey>
    {
        public delegate Expression<Func<T, bool>> Filter<T>(string title);

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TEntity> _logger;
        private readonly IRepository<TEntity, TKey> _repository;
        private readonly string _entityName = typeof(TEntity).Name;

        public Controller(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TEntity> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _repository = _unitOfWork.GetRepository<TEntity, TKey>();
        }

        // Get: {EntityEndpoint}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TDto>>> Get<TDto, TDtoKey>(
                    string? title, bool? inclusion = true, int pageIndex = 1, int pageSize = 10)
                    where TDto : class, IEntity<TKey>
                    where TDtoKey : struct
        {
            bool include = true;

            if (inclusion.HasValue) include = inclusion.Value;

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
            }

            try
            {
                // TODO: Add support for include parameter
                // Generate code to filter by title and pagination
                IEnumerable<TEntity> entities =
                    await _repository.GetPagedAsync(pageIndex, pageSize, t => t.Searchable.Contains(title ?? string.Empty), t => t.OrderBy(e => e.Id));

                if (!entities.Any())
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        _logger.LogWarning($"No {_entityName} found with title containing '{title}'.");
                        return NotFound($"No {_entityName} found with title containing '{title}'.");
                    }

                    _logger.LogWarning("No tournaments found.");
                    return NotFound("No tournaments found.");
                }

                return Ok(_mapper.Map<IEnumerable<TDto>>(entities));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving {_entityName}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: {EntityEndpoint}/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TDto>> Get<TDto, TDtoKey>(TKey id, bool? inclusion)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct
        {
            bool include = true;

            if (inclusion.HasValue) include = inclusion.Value;

            try
            {
                var entity = await _repository.GetAsync(id);

                if (entity is null)
                {
                    _logger.LogWarning($"{_entityName} with ID {id} not found.");
                    return NotFound($"{_entityName} with ID {id} not found.");
                }

                var dto = _mapper.Map<TDto>(entity);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the {_entityName} with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // Post: {EntityEndpoint}
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TDto>> Create<TDto, TDtoKey>(TDto entityDto)
            where TDto : class
            where TDtoKey : struct
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for the TournamentDto.");
                return BadRequest(ModelState);
            }

            var entity = _mapper.Map<TEntity>(entityDto);
            string dtoName = entityDto.GetType().Name;

            if (entity is null)
            {
                _logger.LogWarning($"Mapping error: Could not map {dtoName} to {_entityName}.");
                return StatusCode(500, "Internal server error.");
            }

            // TODO: Support for validating the entity

            try
            {
                await _repository.AddAsync(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrent update error occurred while creating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "A concurrency error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }

            var createdDto = _mapper.Map<TDto>(entity);
            return CreatedAtAction($"Get{_entityName.ToUpper()}", new { id = entity.Id }, createdDto);
        }

        // Put: {EntityEndpoint}/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update<TDto, TDtoKey>(TKey id, TDto entityDto)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct
        {
            string dtoName = entityDto.GetType().Name;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid model state for the {dtoName}.");
                return BadRequest(ModelState);
            }

            var entity = await _repository.GetAsync(id);

            if (entity is null)
            {
                _logger.LogError($"Conflict: Update {_entityName} with ID {id} not found.");
                return NotFound($"{_entityName} with ID {id} not found.");
            }

            _mapper.Map(entityDto, entity);

            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrent update error occurred while updating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "A concurrency error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NoContent();
        }

        // Delete: {EntityEndpoint}/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TKey id)
        {
            var entity = await _repository.GetAsync(id);

            if (entity is null)
            {
                _logger.LogWarning($"{_entityName} with ID {id} not found.");
                return NotFound($"{_entityName} not found.");
            }

            try
            {
                await _repository.RemoveAsync(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrent update error occurred while deleting {_entityName} with ID {id}.");
                return StatusCode(500, "A concurrency error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting {_entityName} with ID {id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NoContent();
        }

        // PATCH: {EntityEndpoint}/{id}
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Patch<TDto, TDtoKey>(TKey id, [FromBody] JsonPatchDocument<TDto> patch)
            where TDto : class, IEntity<TDtoKey>
            where TDtoKey : struct
        {
            if (patch is null)
            {
                _logger.LogWarning("Invalid patch document.");
                return BadRequest("Invalid patch document.");
            }

            var entity = await _repository.GetAsync(id);

            if (entity is null)
            {
                _logger.LogWarning($"{_entityName} with ID {id} not found.");
                return NotFound($"{_entityName} not found.");
            }

            var dto = _mapper.Map<TDto>(entity);


            patch.ApplyTo(dto, error => ModelState.AddModelError(error.Operation.path, error.ErrorMessage));

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid model state for the {_entityName}.");
                return BadRequest(ModelState);
            }

            var patchedEntity = _mapper.Map<TEntity>(dto);

            if (!TryValidateModel(patchedEntity))
            {
                _logger.LogWarning($"Invalid model state when attempting to patch {_entityName} with ID '{entity.Id}'.");
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.UpdateAsync(patchedEntity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrent update error occurred while updating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "A concurrency error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating {_entityName} with ID {entity.Id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NoContent();
        }
    }
}