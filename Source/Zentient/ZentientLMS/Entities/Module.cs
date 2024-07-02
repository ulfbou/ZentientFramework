using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Core.Entities
{
    public class Module
    {
        public int Id { get; set; }

        [Required]
        [MinLength(length: 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        // Computed property
        public string SearchableString => $"{Name} {Description} {StartDate:yyyy-MM-dd} {EndDate:yyyy-MM-dd}";

        // Navigation properties
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
