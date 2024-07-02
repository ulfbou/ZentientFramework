namespace LMS.Core.Entities
{
    public class Discussion : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<Thread> Threads { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}