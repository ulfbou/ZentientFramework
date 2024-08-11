using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zentient.Core;

namespace Zentient.GenericRepository.QueryObjects
{
    public class QueryTemplates<TEntity> where TEntity : class, IEntity
    {
        public QueryHandler<TEntity> GetByIdAsync()
        {
            return async (query, parameters) =>
            {
                if (parameters.Id == null) throw new ArgumentNullException(nameof(parameters.Id), "Id can not be null.");

                var builtQuery = new QueryBuilder<TEntity>()
                    .Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id))
                    .Build();

                var result = await builtQuery.ApplyAsync(query, parameters.Id);
                return (result);
            };
        }

        public QueryHandler<TEntity> GetByIdAsync<TIncludable>() where TIncludable : class, IEntity
        {
            return async (query, parameters) =>
            {
                if (parameters is QueryIncludeParameters<TEntity> includeParameters)
                {
                    if (parameters.Id == null) throw new ArgumentNullException(nameof(parameters.Id), "Id can not be null.");
                    if (includeParameters.IncludeParameters == null) throw new ArgumentNullException(nameof(includeParameters.IncludeParameters), "IncludeParameters can not be null.");

                    var builder = new QueryBuilder<TEntity>()
                        .Where((entity, parameters) => entity.GetIdentifier().Equals(parameters.Id));

                    foreach (var includable in includeParameters.IncludeParameters)
                    {
                        builder = builder.Include(includable);
                    }

                    var builtQuery = builder.Build();

                    var result = await builtQuery.ApplyAsync(query, parameters.Id);
                    return (result);
                }
                else
                {
                    throw new ArgumentException("Invalid parameters type.", nameof(parameters));
                }
            };
        }
    }
}