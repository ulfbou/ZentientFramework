namespace LMS.Core.Entities
{
    public class Module : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<Document> Documents { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}