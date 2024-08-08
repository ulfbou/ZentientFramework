using LMS.Core.Identity;
using System.ComponentModel.DataAnnotations;
using Zentient.Repository;

namespace LMS.Core.Entities
{
    public class Document : BaseEntity<int>
    {
        [Required]
        [MinLength(length: 3)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(length: 3)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [MinLength(length: 3)]
        public string FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;

        // Computed property
        public string SearchableString => $"{Title} {Description} {UploadTime:yyyy-MM-dd}";


        // Navigation properties
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
        public Guid ModuleId { get; set; }
        public Module? Module { get; set; }
        public Guid ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}