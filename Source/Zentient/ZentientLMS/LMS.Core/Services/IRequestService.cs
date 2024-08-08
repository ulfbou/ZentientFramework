using LMS.Core.DTOs;
using LMS.Core.Entities;
using LMS.Core.Identity;
using Zentient.Repository;

namespace LMS.Core.Services
{
    public interface IActivityRequestService2 : IRequestService<Activity, int>
    {
        public Task<IEnumerable<Activity>> GetActivitiesByModuleIdAsync(int moduleId, CancellationToken cancellation = default);
    }
    public interface IApplicationUserRequestService2 : IRequestService<ApplicationUser, string>
    {
        Task<IEnumerable<string>> GetUserRolesAsync(string userId, bool includeRoles = true, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> RemoveUserRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    }
    public interface ICourseRequestService2 : IRequestService<Course, int>
    {
        Task<HttpResponseMessage> UpdateAsync(int id, CourseDTO entity, CancellationToken cancellation = default);
        Task<Course?> GetCourseForUserAsync(string userId, CancellationToken cancellation = default);
    }
    public interface IDocumentRequestService2 : IRequestService<Document, int>
    {
    }
    public interface IModuleRequestService2 : IRequestService<Module, int>
    {
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId, CancellationToken cancellation = default);
    }
    public interface IRequestService<TEntity> : IRequestService<TEntity, int>
        where TEntity : class, IEntity<int>
    { }
    public interface IRequestService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
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
        Task<IEnumerable<TEntity>> GetEntitiesByParentIdAsync(TKey parentId, TKey id, CancellationToken cancellation = default);
        Task<IEnumerable<TEntity>> GetEntitiesByParentIdAsync(TKey parentId, CancellationToken cancellation = default);
    }
}
