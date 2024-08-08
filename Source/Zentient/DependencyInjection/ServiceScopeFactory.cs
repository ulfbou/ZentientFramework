namespace Zentient.DependencyInjection
{
    public class ServiceScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            // Create a new scope
            return new ServiceScope();
        }
    }
}
