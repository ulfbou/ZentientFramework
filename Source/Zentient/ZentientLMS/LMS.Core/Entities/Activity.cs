using LMS.Core.Enums;
using Zentient.Repository;

namespace LMS.Core.Entities
{
    public class Activity : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ActivityType Type { get; set; } = ActivityType.Lecture;
        public DateTime Starts { get; set; } = DateTime.Now;
        public DateTime Ends { get; set; } = DateTime.Now;

        // Computed property
        public string SearchableString => $"{Title} {Description} {Starts:yyyy-MM-dd} {Ends:yyyy-MM-dd}";

        // Navigation properties
        public Guid ModuleId { get; set; }
        public Module? Module { get; set; } = new Module();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}