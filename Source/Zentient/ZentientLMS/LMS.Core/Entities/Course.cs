using LMS.Core.Identity;
using System.ComponentModel.DataAnnotations;
using Zentient.Repository;

namespace LMS.Core.Entities
{
    public class Course : DatableEntity<int>, IDatableEntity<int>
    {
        [Required]
        [MinLength(length: 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Starts { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Ends { get; set; } = DateTime.UtcNow;

        [Required]
        public string CourseCode { get; set; } = string.Empty;

        // Computed property
        public override string Searchable
        {
            get => $"Title:`{Title}` Description:`{Description}` Starts:`{Starts:yyyy-MM-dd}` Ends:`{Ends:yyyy-MM-dd}` CourseCode:`{CourseCode}`";
        }

        // Navigation properties
        public Guid TeacherId { get; set; }
        public ApplicationUser? Teacher { get; set; }
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}