namespace Zentient.Attributes.Injections
{
    /// <summary>
    /// Indicates that a class serves as a service implementation to be registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of service to be registered.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Initializes a new instance of the RegisterServiceAttribute class with the specified service type.
        /// </summary>
        /// <param name="serviceType">The type of service to be registered.</param>
        public RegisterServiceAttribute(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            ServiceType = serviceType;
        }
    }
}
