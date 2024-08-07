namespace Zentient.DependencyInjection
{
    public interface IServiceScopeFactory
    {
        IServiceScope CreateScope();
    }
}