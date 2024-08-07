using LMS.Core.DTOs;
using LMS.Core.Entities;
using LMS.Core.Identity;

using Zentient.Repository;

namespace LMS.Core.Services
{
    public interface IActivityRequestService : IGenericRequestService<Activity, int>
    {
        public Task<IEnumerable<Activity>> GetActivitiesByModuleIdAsync(int moduleId, CancellationToken cancellation = default);
    }
    public interface IApplicationUserRequestService : IGenericRequestService<ApplicationUser, string>
    {
        Task<IEnumerable<string>> GetUserRolesAsync(string userId, bool includeRoles = true, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> RemoveUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    }
    public interface ICourseRequestService : IGenericRequestService<Course, int>
    {
        Task<Course?> GetCourseForUserAsync(string userId, CancellationToken cancellation = default);
    }
    public interface IDocumentRequestService : IGenericRequestService<Document, int>
    {
    }
    public interface IModuleRequestService : IGenericRequestService<Module, int>
    {
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId, CancellationToken cancellation = default);
    }
    public interface IGenericRequestService<TEntity> : IGenericRequestService<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }
    public interface IGenericRequestService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull
    {
        Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellation = default);
        Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default);
        Task<TDto?> GetAsync<TDto>(TKey id, CancellationToken cancellation = default);
        Task<HttpResponseMessage> CreateAsync(TEntity entity, CancellationToken cancellation = default);
        Task<HttpResponseMessage> CreateAsync<TDto>(TDto entity, CancellationToken cancellation = default);
        Task<HttpResponseMessage> UpdateAsync(TEntity entity, CancellationToken cancellation = default);
        Task<HttpResponseMessage> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellation = default);
        Task<HttpResponseMessage> UpdateAsync<TDto>(TKey id, TDto entity, CancellationToken cancellation = default);
        Task<HttpResponseMessage> DeleteAsync(TKey id, CancellationToken cancellation = default);
    }
}
