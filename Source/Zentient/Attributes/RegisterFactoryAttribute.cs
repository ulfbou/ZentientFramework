namespace Zentient.Attributes.Injections
{
    /// <summary>
    /// Indicates that a method serves as a factory for creating instances of a specified service type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterFactoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of service for which the method serves as a factory.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Initializes a new instance of the RegisterFactoryAttribute class with the specified service type.
        /// </summary>
        /// <param name="serviceType">The type of service for which the method serves as a factory.</param>
        public RegisterFactoryAttribute(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            ServiceType = serviceType;
        }
    }
}
