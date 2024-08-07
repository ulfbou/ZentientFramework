using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class Notification : BaseEntity<Guid>
    {
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}