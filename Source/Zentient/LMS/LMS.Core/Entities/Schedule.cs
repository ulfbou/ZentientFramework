namespace LMS.Core.Entities
{
    public class Schedule : BaseEntity<Guid>
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}