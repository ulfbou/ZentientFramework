using System.Linq;
using System.Reflection;

namespace Zentient.DependencyInjection
{
    public static class ConstructorSelector
    {
        public static ConstructorInfo GetBestConstructor(Type implementationType, IServiceProvider serviceProvider)
        {
            var constructors = implementationType.GetConstructors();
            foreach (var constructor in constructors.OrderByDescending(c => c.GetParameters().Length))
            {
                var parameters = constructor.GetParameters();
                if (parameters.All(p => serviceProvider.CanResolve(p.ParameterType)))
                {
                    return constructor;
                }
            }

            throw new InvalidOperationException($"No suitable constructor found for type {implementationType.FullName}");
        }

        public static bool CanResolve(this IServiceProvider serviceProvider, Type type)
        {
            try
            {
                serviceProvider.GetService(type);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}