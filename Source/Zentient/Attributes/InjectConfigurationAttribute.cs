namespace Zentient.Attributes.Injections
{
    /// <summary>
    /// Indicates that a constructor or property should be injected with configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
    public class InjectConfigurationAttribute : Attribute
    {
    }
}
