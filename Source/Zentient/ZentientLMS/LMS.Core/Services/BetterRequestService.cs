using LMS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Web;

namespace LMS.Core.Services
{
    using LMS.Core.Search;
    using System.Linq;

    public class BetterRequestService<TEntity, TKey> : IBetterRequestService<TEntity, TKey> where TEntity : class
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointPath;

        public BetterRequestService(HttpClient httpClient, IConfiguration configuration, string serviceName)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var serviceConfig = configuration.GetSection($"RequestServices:{serviceName}");
            _endpointPath = serviceConfig.GetValue<string>("EndpointPath") ?? throw new InvalidOperationException($"EndpointPath not found for service '{serviceName}'.");
        }


        public Task<(IEnumerable<TEntity> items, int totalCount, object totalPages)> SearchAsync<T>(SearchParameters searchParameters)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBulkAsync<T1, T2>(IEnumerable<object> updateDtos)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(_endpointPath, cancellationToken);
                return response ?? Enumerable.Empty<TEntity>();
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error fetching data from {_endpointPath}: {ex.Message}", ex);
            }
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_endpointPath, entity, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the created entity from the response body.
                    return await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken: cancellationToken);
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error creating entity at {_endpointPath}: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error creating entity at {_endpointPath}: {ex.Message}", ex);
            }
        }

        public async Task<TReadDto> CreateAsync<TCreateDto, TReadDto>(TCreateDto createDto, CancellationToken cancellationToken = default) where TReadDto : class
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_endpointPath, createDto, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the read DTO from the response body.
                    return await response.Content.ReadFromJsonAsync<TReadDto>(cancellationToken: cancellationToken);
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error creating entity with DTO at {_endpointPath}: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error creating entity with DTO at {_endpointPath}: {ex.Message}", ex);
            }
        }

        public async Task<TReadDto> UpdateAsync<TKey, TUpdateDto, TReadDto>(TKey id, TUpdateDto updateDto, CancellationToken cancellationToken = default) where TReadDto : class
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_endpointPath}/{id}", updateDto, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the read DTO from the response body.
                    return await response.Content.ReadFromJsonAsync<TReadDto>(cancellationToken: cancellationToken);
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error updating entity with DTO at {_endpointPath}/{id}: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error updating entity with DTO at {_endpointPath}/{id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_endpointPath}/{id}", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Optionally, you can return more detailed information based on the response.
                    return true;
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error deleting entity at {_endpointPath}/{id}: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error deleting entity at {_endpointPath}/{id}: {ex.Message}", ex);
            }
        }

        // Bulk operations
        // - partial failures in bulk operations?
        // - return more detailed information based on the response?
        // - return detailed results for each entity in bulk operations?
        // - error handling for bulk operations?


        public async Task<IEnumerable<TReadDto>> CreateBulkAsync<TCreateDto, TReadDto>(IEnumerable<TCreateDto> createDtos, CancellationToken cancellationToken = default) where TReadDto : class
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_endpointPath}/bulk", createDtos, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the list of read DTOs from the response body.
                    return await response.Content.ReadFromJsonAsync<IEnumerable<TReadDto>>(cancellationToken: cancellationToken);
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error creating entities in bulk at {_endpointPath}/bulk: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error creating entities in bulk at {_endpointPath}/bulk: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBulkAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{_endpointPath}/bulk")
                {
                    Content = JsonContent.Create(ids)
                };
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns true for successful bulk deletion.
                    return true;
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error deleting entities in bulk at {_endpointPath}/bulk: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error deleting entities in bulk at {_endpointPath}/bulk: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TReadDto>> UpdateBulkAsync<TKey, TUpdateDto, TReadDto>(IEnumerable<KeyValuePair<TKey, TUpdateDto>> updates, CancellationToken cancellationToken = default) where TReadDto : class
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_endpointPath}/bulk", updates, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the list of read DTOs from the response body.
                    return await response.Content.ReadFromJsonAsync<IEnumerable<TReadDto>>(cancellationToken: cancellationToken);
                }
                else
                {
                    // Handle non-success status codes by throwing an ApiException with details.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error updating entities in bulk at {_endpointPath}/bulk: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error updating entities in bulk at {_endpointPath}/bulk: {ex.Message}", ex);
            }
        }


        // Pagination operations
        // - pagination metadata in response headers or body?
        // - GetAsync with pagination parameters?
        // - GetAsync with search query?
        // - GetAsync with both pagination parameters and search query?
        public async Task<(IEnumerable<TEntity> Items, int TotalCount, int TotalPages)> GetAsync(PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["pageNumber"] = parameters.PageNumber.ToString();
            query["pageSize"] = parameters.PageSize.ToString();
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                query["sortBy"] = parameters.SortBy;
                query["sortDesc"] = parameters.SortDescending.ToString();
            }
            // Encode complex filters into the query string
            if (parameters.Filters.Any())
            {
                var filterJson = JsonConvert.SerializeObject(parameters.Filters);
                query["filters"] = filterJson;
            }

            var uriBuilder = new UriBuilder($"{_httpClient.BaseAddress}{_endpointPath}")
            {
                Query = query.ToString()
            };

            try
            {
                var response = await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var items = await response.Content.ReadFromJsonAsync<IEnumerable<TEntity>>(cancellationToken: cancellationToken);
                    int totalCount = int.Parse(response.Headers.GetValues("X-Total-Count").FirstOrDefault() ?? "0");
                    int totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

                    return (items, totalCount, totalPages);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApiException($"Error fetching data with complex filters from {_endpointPath}: {response.StatusCode}, {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Error fetching data with complex filters from {_endpointPath}: {ex.Message}", ex);
            }
        }
    }
}
