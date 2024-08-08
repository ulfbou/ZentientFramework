namespace LMS.Core.Entities
{
    public class Document : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid ModuleId { get; set; }
        public Module Module { get; set; }
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}