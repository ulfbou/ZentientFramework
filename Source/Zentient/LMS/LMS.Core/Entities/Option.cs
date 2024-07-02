namespace LMS.Core.Entities
{
    public class Option : BaseEntity<Guid>
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
}