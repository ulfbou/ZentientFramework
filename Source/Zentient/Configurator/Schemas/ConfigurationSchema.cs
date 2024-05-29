using FluentValidation.Results;
using System.Net;

namespace Zentient.Configurator.Schemas
{
    /// <summary>
    /// Defines the structure and validation rules for configuration data.
    /// </summary>
    public class ConfigurationSchema : Schema
    {
        public bool TryValidate(Configuration configuration, out ValidationResult validationResult)
        {
            var errors = new List<ValidationFailure>();

            foreach (var kvp in configuration.Data)
            {
                if (!Properties.ContainsKey(kvp.Key))
                {
                    errors.Add(new ValidationFailure(kvp.Key, "Property not found in schema"));
                    continue;
                }

                if (!IsValidType(kvp.Value, Properties[kvp.Key].Type))
                {
                    errors.Add(new ValidationFailure(kvp.Key, $"Invalid type for property {kvp.Key}"));
                }
            }

            validationResult = new ValidationResult(errors);
            return validationResult.IsValid;
        }

        private bool IsValidType(object value, string expectedType)
        {
            switch (expectedType.ToLower())
            {
                case "string":
                    return value is string;
                case "int":
                    return value is int;
                case "bool":
                    return value is bool;
                case "datetime":
                    return value is DateTime;
                case "decimal":
                    return value is decimal;
                case "double":
                    return value is double;
                case "float":
                    return value is float;
                case "long":
                    return value is long;
                case "short":
                    return value is short;
                case "byte":
                    return value is byte;
                case "char":
                    return value is char;
                case "sbyte":
                    return value is sbyte;
                case "uint":
                    return value is uint;
                case "ulong":
                    return value is ulong;
                case "ushort":
                    return value is ushort;
                case "guid":
                    return value is Guid;
                case "timespan":
                    return value is TimeSpan;
                case "uri":
                    return value is Uri;
                case "version":
                    return value is Version;
                default:
                    return false;
            }
        }
    }
}
