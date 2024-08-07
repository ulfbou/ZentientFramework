namespace LMS.Core.Entities
{
    public class Question : BaseEntity<Guid>
    {
        public string Text { get; set; }
        public ICollection<Option> Options { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}