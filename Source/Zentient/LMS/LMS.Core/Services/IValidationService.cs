using Zentient.Repository;

namespace LMS.Core.Services
{
    public interface IValidationService<TEntity>
    {
        Task<OperationResult> ValidateAsync(TEntity entity);
    }
    public interface IExtendedUnitOfWork : IUnitOfWork
    {
        //ICourseRepository Courses { get; }
        //IUserRepository Users { get; }
        //IModuleRepository Modules { get; }

        Task ExecuteBusinessTransactionAsync(Func<Task> action, CancellationToken cancellation = default);
    }
}