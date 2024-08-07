namespace LMS.Core.Entities
{
    public class Assignment : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }
}