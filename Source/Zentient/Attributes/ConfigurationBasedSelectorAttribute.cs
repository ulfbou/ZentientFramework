using System.Reflection.Metadata;

namespace Zentient.Attributes.Injections
{
    /// <summary>
    /// Specifies the environment in which a class or method should be included or excluded based on a configuration key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ConfigurationBasedSelectorAttribute : Attribute
    {
        private string _configurationKey = null!;

        /// <summary>
        /// Gets or sets the configuration key used for environment selection.
        /// </summary>
        /// <value>
        /// The configuration key.
        /// </value>
        public string ConfigurationKey
        {
            get => _configurationKey;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(ConfigurationKey), "ConfigurationKey cannot be null.");
                }

                _configurationKey = value;
            }
        }
    }
}
