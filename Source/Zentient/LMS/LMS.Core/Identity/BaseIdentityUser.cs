using LMS.Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace LMS.Core.Identity
{
    public abstract class BaseIdentityUser : IdentityUser<Guid>, IEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
