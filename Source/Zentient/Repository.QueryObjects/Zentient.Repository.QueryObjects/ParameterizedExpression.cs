using System.Linq.Expressions;

namespace Zentient.Repository.QueryObjects
{
    public class ParameterizedExpression<T>
    {
        public Expression<Func<object[], Expression<Func<T, bool>>>> Expression { get; }

        public ParameterizedExpression(Expression<Func<object[], Expression<Func<T, bool>>>> expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }
    }
}