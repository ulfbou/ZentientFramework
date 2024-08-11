using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository.Tests
{
    public static class TestQueryObjects
    {
        // TODO: GetGenericSelectQuery

        // GetGenericQuery
        public static IQuery<TEntity> GetGenericQuery<TEntity>(
            Expression<Func<TEntity, QueryParameters, bool>>? whereExpression = null,
            Expression<Func<TEntity, object>>? orderByExpression = null,
            bool? ordering = null,
            Expression<Func<TEntity, object>>? groupByExpression = null,
            int? skip = null,
            int? take = null,
            Expression<Func<TEntity, object>>? _includeExpression = null)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();
            if (whereExpression != null)
            {
                builder.Where(whereExpression);
            }

            if (orderByExpression != null)
            {
                builder.OrderBy(orderByExpression, ordering ?? false);
            }

            if (groupByExpression != null)
            {
                builder.GroupBy(groupByExpression);
            }

            if (skip.HasValue)
            {
                builder.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                builder.Take(take.Value);
            }

            if (_includeExpression != null)
            {
                builder.Include(_includeExpression);
            }

            return builder.Build();
        }

        public static IQuery<TEntity> GetQuery<TEntity>()
            where TEntity : class, IEntity
        {
            IQueryBuilder<TEntity> builder = new QueryBuilder<TEntity>();
            return builder.Build();
        }

        public static IQuery<TEntity> GetEntityByIdQuery<TEntity>()
            where TEntity : class, IEntity
        {
            IQueryBuilder<TEntity> builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            return builder.Build();
        }

        public static IQuery<TEntity> GetOrderedBy<TEntity>(bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.OrderBy(entity => entity.Name, descending);

            return builder.Build();
        }

        // GetTakeQuery
        public static IQuery<TEntity> GetTakeQuery<TEntity>(int take)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Take(take);

            return builder.Build();
        }

        // GetSkipQuery
        public static IQuery<TEntity> GetSkipQuery<TEntity>(int skip)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Skip(skip);

            return builder.Build();
        }

        // GetSkipTakeQuery
        public static IQuery<TEntity> GetSkipTakeQuery<TEntity>(int skip, int take)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Skip(skip);
            builder.Take(take);

            return builder.Build();
        }

        // GetGroupByQuery
        public static IQuery<TEntity> GetGroupByCategoryQuery<TEntity>()
            where TEntity : class, IEntity, ICategoryEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.GroupBy(entity => entity.Category);

            return builder.Build();
        }

        // GetIncludeQuery
        public static IQuery<TEntity> GetIncludeQuery<TEntity>()
            where TEntity : class, IEntity, ICategoryEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Include(entity => entity.Category);

            return builder.Build();
        }

        // GetWhereIncludeQuery
        public static IQuery<TEntity> GetWhereIncludeQuery<TEntity>()
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.Include(entity => entity.GetIdentifier());

            return builder.Build();
        }

        // GetWhereOrderByQuery
        public static IQuery<TEntity> GetWhereOrderByQuery<TEntity>(bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.OrderBy(entity => entity.Name, descending);

            return builder.Build();
        }

        // GetWhereOrderBySkipTakeQuery
        public static IQuery<TEntity> GetWhereOrderBySkipTakeQuery<TEntity>(int skip, int take, bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.OrderBy(entity => entity.Name, descending);
            builder.Skip(skip);
            builder.Take(take);

            return builder.Build();
        }

        // GetWhereOrderByGroupByQuery
        public static IQuery<TEntity> GetWhereOrderByGroupByQuery<TEntity>(bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.OrderBy(entity => entity.Name, descending);
            builder.GroupBy(entity => entity.GetIdentifier());

            return builder.Build();
        }

        // GetWhereOrderByGroupByIncludeQuery
        public static IQuery<TEntity> GetWhereOrderByGroupByIncludeQuery<TEntity>(bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.OrderBy(entity => entity.Name, descending);
            builder.GroupBy(entity => entity.GetIdentifier());
            builder.Include(entity => entity.GetIdentifier());

            return builder.Build();
        }

        // GetWhereOrderByGroupByIncludeSkipTakeQuery
        public static IQuery<TEntity> GetWhereOrderByGroupByIncludeSkipTakeQuery<TEntity>(int skip, int take, bool descending = false)
            where TEntity : class, IEntity, INamedEntity
        {
            var builder = new QueryBuilder<TEntity>();
            builder.Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));
            builder.OrderBy(entity => entity.Name, descending);
            builder.GroupBy(entity => entity.GetIdentifier());
            builder.Include(entity => entity.GetIdentifier());
            builder.Skip(skip);
            builder.Take(take);

            return builder.Build();
        }
    }
}