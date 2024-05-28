using FluentValidation;
using Zentient.Configurator.Schemas;

namespace Zentient.Configurator.Validation
{
    /// <summary>
    /// Centralizes validation logic for configurations.
    /// </summary>
    public class ConfigurationValidator
    {
        public bool Validate(Schema schema, Configuration config)
        {
            if (schema is ConfigurationSchema configurationSchema)
            {
                return configurationSchema.TryValidate(config, out _);
            }
            return false;
            //var validator = new InlineValidator<Configuration>();
            //foreach (var property in schema.Properties)
            //{
            //    validator.RuleFor(c => c.Data[property.Key]).NotEmpty();
            //}
            //var result = validator.Validate(config);
            //return result.IsValid;
        }
    }
}