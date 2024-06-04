using Zentient.Configurator;

namespace Zentient.Configurator
{
    /// <summary>
    /// Interface for classes that support custom configurations.
    /// </summary>
    public interface IConfigurable
    {
        void ApplyConfiguration(Configuration config);
    }
}
