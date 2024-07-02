namespace LMS.Core.Entities
{
    public class Exam : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}