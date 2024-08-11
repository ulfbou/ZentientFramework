using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.GenericRepository.QueryObjects
{
    public class QueryParametersTypeMismatchException : Exception
    {
        public Type? ExpectedType { get; set; }
        public Type? ActualType { get; set; }

        public QueryParametersTypeMismatchException(string message) : base(message) { }

        public QueryParametersTypeMismatchException(Type expectedType, Type actualType)
            : base($"Type parameter mismatch: Expected type '{expectedType}, actual type '{actualType}'.")
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}