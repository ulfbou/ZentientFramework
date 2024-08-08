namespace LMS.Core.Entities
{
    public interface IEntity<TKey> : IIdentifiable<TKey> where TKey : notnull
    {
        public DateTime CreatedAt { get; set; }
    }
}
