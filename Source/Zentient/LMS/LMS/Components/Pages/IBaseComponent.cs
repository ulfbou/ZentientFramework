using LMS.Core.Entities;

namespace LMS.Components.Pages
{
    public interface IBaseComponent<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : struct
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);

        event Func<Task> OnCreate;
        event Func<Task> OnUpdate;
        event Func<Task> OnDelete;

        Task<bool> HasRoleAsync(string role);
        void NavigateTo(string url);

        void SetLoading(bool isLoading);
        void SetError(string message);
        void SetSuccess(string message);
    }
}
