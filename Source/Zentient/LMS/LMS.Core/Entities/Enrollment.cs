using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class Enrollment : BaseEntity<Guid>
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}