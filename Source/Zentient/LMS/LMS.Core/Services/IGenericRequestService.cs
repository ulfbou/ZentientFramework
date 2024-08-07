using LMS.Core.Entities;
using LMS.Core.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LMS.Core.Services
{
    public interface IGenericRequestService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        Task<IEnumerable<TEntity>> GetAsync(CancellationToken token);
    }
}
