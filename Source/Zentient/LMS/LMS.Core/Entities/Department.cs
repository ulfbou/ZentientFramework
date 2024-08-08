using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class Department : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
    }
}