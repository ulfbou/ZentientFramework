using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.GenericRepository.QueryObjects
{
    public class QueryNameParameter : QueryParameters
    {
        public string Name { get; set; }

        public QueryNameParameter(
            string name,
            string? id = null,
            object[]? parameters = null)
            : base(id, parameters)
        {
            Name = name;
        }
    }

    public class QueryIncludeParameters<TEntity> : QueryParameters where TEntity : class, IEntity
    {
        public Expression<Func<TEntity, object>>[] IncludeParameters { get; set; }

        public QueryIncludeParameters(
            string? id = null,
            object[]? parameters = null,
            params Expression<Func<TEntity, object>>[] includeParameters)
            : base(id, parameters)
        {
            IncludeParameters = includeParameters;
        }
    }

    public class QueryParameters
    {
        public string? Id { get; set; }
        public object[]? Parameters { get; set; }

        public QueryParameters(string? id = null, object[]? parameters = null)
        {
            Id = id;
            Parameters = parameters;
        }

        public QueryParameters(object[] parameters)
        {
            Parameters = parameters;
        }
    }
}