using IdentityModel;

using LMS.Core.DTOs;
using LMS.Core.Entities;
using LMS.Core.Identity;

using Microsoft.Extensions.Configuration;

using System.Net.Http;
using System.Net.Http.Json;

using Zentient.Repository;

namespace LMS.Core.Services
{
    public class ActivityRequestService : GenericRequestService<Activity, int>, IActivityRequestService
    {
        public ActivityRequestService(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration, "Activity")
        { }

        public async Task<IEnumerable<Activity>> GetActivitiesByModuleIdAsync(int moduleId, CancellationToken cancellation = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Activity>>($"{_endpointPath}/Module/{moduleId}");
        }
    }

    public class CourseRequestService : GenericRequestService<Course, int>, ICourseRequestService
    {
        public CourseRequestService(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration, "Course")
        { }

        public Task<Course?> GetCourseForUserAsync(string userId, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Course>> GetCoursesByUserIdAsync(int userId, CancellationToken cancellation = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{_endpointPath}/User/{userId}");
        }
    }

    public class DocumentRequestService : GenericRequestService<Document, int>, IDocumentRequestService
    {
        public DocumentRequestService(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration, "Document")
        { }

        public async Task<IEnumerable<Document>> GetDocumentsByModuleIdAsync(int moduleId, CancellationToken cancellation = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Document>>($"{_endpointPath}/Module/{moduleId}");
        }
    }

    public class ModuleRequestService : GenericRequestService<Module, int>, IModuleRequestService
    {
        public ModuleRequestService(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration, "Module")
        { }

        public async Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId, CancellationToken cancellation = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Module>>($"{_endpointPath}/Course/{courseId}");
        }
    }

    public class ApplicationUserRequestService : GenericRequestService<ApplicationUser, string>, IApplicationUserRequestService
    {
        public ApplicationUserRequestService(HttpClient httpClient, IConfiguration configuration)
            : base(httpClient, configuration, "ApplicationUser")
        { }

        public async Task<HttpResponseMessage> AddUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.PostAsJsonAsync($"{_endpointPath}/{userId}/Role/{role}", null, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to add the role '{role}' to the user with ID '{userId}'.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to add the role '{role}' to the user with ID '{userId}' was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to add the role '{role}' to the user with ID '{userId}'.", ex);
            }
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(string userId, bool includeRoles = true, CancellationToken cancellationToken = default)
        {

        }

        public Task<HttpResponseMessage> RemoveUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericRequestService<TEntity> : GenericRequestService<TEntity, int>, IGenericRequestService<TEntity>
        where TEntity : class, IEntity<int>
    {
        public GenericRequestService(HttpClient httpClient, IConfiguration configuration, string entityName)
            : base(httpClient, configuration, entityName)
        { }
    }
    public class GenericRequestService<TEntity, TKey> : IGenericRequestService<TEntity, TKey> where TEntity : class, IEntity<TKey>
            where TKey : notnull
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpointPath;

        public GenericRequestService(HttpClient httpClient, IConfiguration configuration, string serviceName)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            _httpClient = httpClient;

            var serviceConfig = configuration.GetSection($"RequestServices:{serviceName}");
            _endpointPath = serviceConfig.GetValue<string>("EndpointPath") ?? throw new InvalidOperationException($"EndpointPath not found for service '{serviceName}'.");
        }

        public async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellation = default)
        {
            try
            {
                var response = await _httpClient.GetAsync(_endpointPath, cancellation);
                IEnumerable<TEntity>? result = null;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<IEnumerable<TEntity>>();
                }
                return result ?? Enumerable.Empty<TEntity>();
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to retrieve data from {_endpointPath}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to {_endpointPath} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to retrieve data from {_endpointPath}.", ex);
            }
        }

        public async Task<TDto?> GetAsync<TDto>(TKey id, CancellationToken cancellation = default)
        {
            return await _httpClient.GetFromJsonAsync<TDto>($"{_endpointPath}/{id}");
        }

        public async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpointPath}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TEntity>();
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to retrieve data from {_endpointPath}/{id}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to {_endpointPath}/{id} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to retrieve data from {_endpointPath}/{id}.", ex);
            }
            return default;
        }

        public async Task<HttpResponseMessage> CreateAsync<TDto>(TDto dto, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PostAsJsonAsync(_endpointPath, dto, cancellation);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to create a new {typeof(TDto).Name}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to create a new {typeof(TDto).Name} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to create a new {typeof(TDto).Name}.", ex);
            }
        }

        public async Task<HttpResponseMessage> CreateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PostAsJsonAsync(_endpointPath, entity, cancellation);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to create a new {typeof(TEntity).Name}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to create a new {typeof(TEntity).Name} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to create a new {typeof(TEntity).Name}.", ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            return await UpdateAsync(entity.Id, entity, cancellation);
        }

        public async Task<HttpResponseMessage> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PutAsJsonAsync($"{_endpointPath}/{entity.Id}", entity, cancellation);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to update the {typeof(TEntity).Name}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to update the {typeof(TEntity).Name} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to update the {typeof(TEntity).Name}.", ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateAsync<TDto>(TKey id, TDto entity, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PutAsJsonAsync($"{_endpointPath}/{id}", entity, cancellation);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to update the {typeof(TEntity).Name}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to update the {typeof(TEntity).Name} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to update the {typeof(TEntity).Name}.", ex);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(TKey id, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.DeleteAsync($"{_endpointPath}/{id}", cancellation);
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"An error occurred while attempting to delete the {typeof(TEntity).Name}.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException($"The request to delete the {typeof(TEntity).Name} was cancelled.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while attempting to delete the {typeof(TEntity).Name}.", ex);
            }
        }
    }
}
