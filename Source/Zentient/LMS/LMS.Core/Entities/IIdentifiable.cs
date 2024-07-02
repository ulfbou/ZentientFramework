namespace LMS.Core.Entities
{
    public interface IIdentifiable<TKey> where TKey : notnull
    {
        public TKey Id { get; set; }
    }
}
