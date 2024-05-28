using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Zentient.Configurator;

namespace Zentient.Configurator.Schemas
{
    /// <summary>
    /// Defines the structure and validation rules for configuration data.
    /// </summary>
    public class ConfigurationSchema : Schema
    {
        public bool TryValidate(Configuration configuration, out ValidationResult validationResult)
        {
            var validator = new InlineValidator<Configurator>();

            try
            {
                foreach (var property in Properties)
                {
                    if (!configuration.Data.ContainsKey(property.Key))
                    {
                        validationResult = new ValidationResult(new[] { new ValidationFailure(property.Key, "Property not found") });
                        return false;
                    }

                    validator.RuleFor(c => c.Data[property.Key]).NotEmpty(); // Simplified validation
                }

                validationResult = validator.Validate(configuration);
                return validationResult.IsValid;
            }
            catch (Exception ex)
            {
                // Log exception
                validationResult = new ValidationResult(new[] {
                    new ValidationFailure(configuration.SchemaName, $"Exception thrown with `{configuration.SchemaName}: {ex.InnerException}.")
                });
                return false;
            }
        }
    }
}
