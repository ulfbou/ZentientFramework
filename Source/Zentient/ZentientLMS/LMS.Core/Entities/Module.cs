using System.ComponentModel.DataAnnotations;
using Zentient.Repository;

namespace LMS.Core.Entities
{
    public class Module : BaseEntity<int>
    {
        [Required]
        [MinLength(length: 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public string Description { get; set; } = string.Empty;

        public DateTime Starts { get; set; } = DateTime.UtcNow;
        public DateTime Ends { get; set; } = DateTime.UtcNow;

        // Computed property
        public string SearchableString => $"{Title} {Description} {Starts:yyyy-MM-dd} {Ends:yyyy-MM-dd}";

        // Navigation properties
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public ICollection<Document> Documents { get; set; } = [];
        public ICollection<Activity> Activities { get; set; } = [];
    }
}
