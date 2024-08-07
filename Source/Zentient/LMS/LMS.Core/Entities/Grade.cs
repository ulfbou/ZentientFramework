using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class Grade : BaseEntity<Guid>
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public double Value { get; set; }
        public DateTime DateAwarded { get; set; }
    }
}