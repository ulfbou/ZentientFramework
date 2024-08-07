using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class Course : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Discussion> Discussions { get; set; }
    }
}