using LMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zentient.Repository;

namespace LMS.Core.Repository
{
    public interface IValidatedRepository<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        IValidationService<TEntity> Validator { get; }
    }
}
