namespace Zentient.Attributes.Injections
{
    /// <summary>
    /// Specifies the environment in which a class or method should be included or excluded.
    /// </summary>
    /// <remarks>
    /// Use this attribute to conditionally include or exclude classes or methods based on the environment name provided.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnvironmentSelectorAttribute : Attribute
    {
        private string _environmentName = null!;

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        /// <value>
        /// The name of the environment.
        /// </value>
        public string EnvironmentName
        {
            get => _environmentName;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(EnvironmentName));
                }

                _environmentName = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode for environment selection.
        /// </summary>
        /// <value>
        /// The mode of environment selection, either Include or Exclude.
        /// </value>
        public EnvironmentSelectorMode Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentSelectorAttribute"/> class.
        /// </summary>
        /// <param name="environmentName">The name of the environment.</param>
        /// <param name="mode">The mode of environment selection, with a default of Include.</param>
        public EnvironmentSelectorAttribute(string environmentName, EnvironmentSelectorMode mode = EnvironmentSelectorMode.Include)
        {
            EnvironmentName = environmentName;
            Mode = mode;
        }
    }
}
