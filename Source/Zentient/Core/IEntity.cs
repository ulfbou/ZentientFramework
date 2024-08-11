namespace Zentient.Core
{
    public interface IEntity<TIdentifier> where TIdentifier : IEquatable<TIdentifier>
    {
        TIdentifier GetIdentifier();
    }

    public interface IEntity : IEntity<string> { }
}