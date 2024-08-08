using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [MinLength(length: 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public string Description { get; set; } = string.Empty;

        public DateTime UploadTime { get; set; } = DateTime.UtcNow;

        // TODO: Add DocumentUrl and DocumentType?
        // public string Url { get; set; }
        // public string Type { get; set; }

        // Computed property
        public string SearchableString => $"{Name} {Description} {UploadTime:yyyy-MM-dd}";

        // Navigation properties
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
        public int? ModuleId { get; set; }
        public Module? Module { get; set; }
        public int? ActivityId { get; set; }
        public Activity? Activity { get; set; }
    }
}