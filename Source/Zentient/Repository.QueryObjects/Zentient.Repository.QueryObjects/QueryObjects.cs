using System;
using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.Repository.QueryObjects
{
    public static class QueryObjects
    {
        public static IQuery<TEntity> GetEntityByIdQuery<TEntity>() where TEntity : class, IEntity
        {
            IQueryBuilder<TEntity> builder = new QueryBuilder<TEntity>();

            return builder.Build();
        }
    }
}