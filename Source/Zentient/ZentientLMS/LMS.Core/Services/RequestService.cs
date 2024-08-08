using LMS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Zentient.Repository;

namespace LMS.Core.Services
{
    public class RequestService<TEntity> : RequestService<TEntity, Guid>
        where TEntity : class, IEntity<Guid>, IEquatable<TEntity>, new()
    {
        public RequestService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration) { }
    }

    public class RequestService<TEntity, TKey> : IRequestService<TEntity, TKey>
        where TEntity : class, IEntity<TKey> where TKey : notnull, IEquatable<TKey>
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public RequestService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiUrl = $"{configuration["ApiUrl"]}/{typeof(TEntity).Name.ToLower()}";
        }

        public async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = null;
            string? jsonResponse = null;

            try
            {
                response = await _httpClient.GetAsync(_apiUrl, cancellation);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to retrieve entities.", ex);
            }

            var result = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(jsonResponse);
            return result ?? throw new ApiException("Failed to deserialize response.");
        }

        public async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = null;
            string? jsonResponse = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                response = await _httpClient.GetAsync($"{_apiUrl}/{id}", cancellation);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to retrieve entity.", ex);
            }

            var result = JsonConvert.DeserializeObject<TEntity>(jsonResponse);
            return result ?? throw new ApiException("Failed to deserialize response.");
        }

        public async Task<TDto?> GetAsync<TDto>(TKey id, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = null;
            string? jsonResponse = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                response = await _httpClient.GetAsync($"{_apiUrl}/{id}", cancellation);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to retrieve entity.", ex);
            }

            var result = JsonConvert.DeserializeObject<TDto>(jsonResponse);
            return result ?? throw new ApiException("Failed to deserialize response.");
        }

        public async Task<HttpResponseMessage> CreateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PostAsync(_apiUrl, content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException("Failed to create entity.");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to create entity.", ex);
            }
        }

        public async Task<HttpResponseMessage> CreateAsync<TDto>(TDto entity, CancellationToken cancellation = default)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PostAsync(_apiUrl, content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException("Failed to create entity.");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to create entity.", ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PutAsync(_apiUrl, content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException("Failed to update entity.");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to update entity.", ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellation = default)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException($"Failed to update entity with ID {id}. Status code: {response.StatusCode}");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to update entity.", ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateAsync<TDto>(TKey id, TDto entity, CancellationToken cancellation = default)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content, cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException($"Failed to update entity with ID {id}. Status code: {response.StatusCode}");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to update entity.", ex);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(TKey id, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}", cancellation);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException($"Failed to delete entity with ID {id}. Status code: {response.StatusCode}");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to delete entity.", ex);
            }
        }

        public async Task<IEnumerable<TEntity>> GetEntitiesByParentIdAsync(TKey parentId, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = null;
            string? jsonResponse = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                response = await _httpClient.GetAsync($"{_apiUrl}/parent/{parentId}", cancellation);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to retrieve entities.", ex);
            }

            var result = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(jsonResponse);
            return result ?? throw new ApiException("Failed to deserialize response.");
        }

        public async Task<IEnumerable<TEntity>> GetEntitiesByParentIdAsync(TKey parentId, TKey id, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = null;
            string? jsonResponse = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                response = await _httpClient.GetAsync($"{_apiUrl}/parent/{parentId}/{id}", cancellation);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new ApiException("Failed to retrieve entities.", ex);
            }

            var result = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(jsonResponse);
            return result ?? throw new ApiException("Failed to deserialize response.");
        }

    }
}
