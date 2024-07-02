namespace LMS.Core.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ActivityType Type { get; set; } = ActivityType.Lecture;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;

        // Computed property
        public string SearchableString => $"{Name} {Description} {StartDate:yyyy-MM-dd} {EndDate:yyyy-MM-dd}";

        // Navigation properties
        public int ModuleId { get; set; }
        public Module Module { get; set; } = new Module();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        public int ModuleID { get; set; }

        [JsonIgnore]
        public Module Module { get; set; }

    }
}