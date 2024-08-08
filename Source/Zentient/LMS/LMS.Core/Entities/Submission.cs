using LMS.Core.Identity;

namespace LMS.Core.Entities
{
    public class ResearchPaper : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string FilePath { get; set; }
        public DateTime PublicationDate { get; set; }
        public Guid AuthorId { get; set; }
        public Teacher Author { get; set; }
    }
    public class Submission
    {
    }
}