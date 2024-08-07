using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LMS.Core.Entities
{
    public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : notnull
    {
        public TKey Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
