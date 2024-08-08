using LMS.Core.Entities;

namespace LMS.Core.Identity
{
    public class Student : BaseIdentityUser
    {
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}
