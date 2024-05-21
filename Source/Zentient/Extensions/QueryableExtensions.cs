//
// QueryableExtensions.cs
//
// Description: Provides extension methods for working with IQueryables.
//
// MIT License
//
// Copyright (c) 2024 Ulf Bourelius
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System.Linq.Expressions;

namespace Zentient.Extensions;

/// <summary>
/// Provides extension methods for working with IQueryables.
/// </summary>
public static class QueryableExtensions
{

    /// <summary>
    /// Paginates the query results by skipping a specified number of elements and then taking a specified number of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="pageNumber">The page number (1-based index).</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>A sequence of elements that is a subset of the source query.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 0) throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}");
        if (pageSize < 0) throw new ArgumentOutOfRangeException($"{nameof(pageSize)} is negative.");

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Sorts the elements of the query in ascending or descending order based on the specified property name.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="ascending">True to sort in ascending order; false to sort in descending order.</param>
    /// <returns>A sorted query.</returns>
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("Property name must not be empty", nameof(propertyName));
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);
        string methodName = ascending ? "OrderBy" : "OrderByDescending";
        var methodCallExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(lambda)
        );
        return source.Provider.CreateQuery<T>(methodCallExpression);
    }

    /// <summary>
    /// Applies a filter to the query if a condition is true.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="predicate">The filter predicate to apply if the condition is true.</param>
    /// <returns>A filtered query.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Projects each element of the query sequence to an <see cref="IEnumerable{TSource}"/> and flattens the resulting sequences into one sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the projected sequence.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="selector">A transform function to apply to each element of the source query.</param>
    /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking the transform function on each element of the source query.</returns>
    public static IQueryable<TResult> SelectManyDynamic<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
    {
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));
        var methodCallExpression = Expression.Call(
            typeof(Queryable),
            "SelectMany",
            new[] { typeof(TSource), typeof(TResult) },
            source.Expression,
            selector
        );
        return source.Provider.CreateQuery<TResult>(methodCallExpression);
    }

    /// <summary>
    /// Specifies related entities to include in the query results.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="includes">A list of related entities to include in the query results.</param>
    /// <returns>A query with the specified related entities included.</returns>
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
    {
        if (includes == null)
            return query;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
}
