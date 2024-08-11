using System.Linq.Expressions;

namespace Zentient.Repository.QueryObjects
{
    public interface IQuery<T>
    {
        Task<IEnumerable<T>> ApplyAsync(IQueryable<T> query, object[] parameters);
    }
}