using LMS.Core.Entities;

namespace LMS.Core.Identity
{
    public class Teacher : BaseIdentityUser
    {
        public ICollection<Course> Courses { get; set; }
        public ICollection<Department> Departments { get; set; }
    }
}
