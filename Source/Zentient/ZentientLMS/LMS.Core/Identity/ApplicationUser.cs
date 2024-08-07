using LMS.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Identity
{
    public class ApplicationUser : BaseIdentityUser
    {
        [Required]
        [MinLength(length: 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 6)]
        public string Password { get; set; } = string.Empty;

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
        public string SearchableString => $"{FirstName} {LastName} {Email}";

        // Navigation properties
        public Course Course { get; set; } = new Course();
        public ICollection<Document> Documents { get; set; } = [];
        public ICollection<Course> Courses { get; set; } = [];
        public ICollection<Role> Roles { get; set; } = [];
    }
}
