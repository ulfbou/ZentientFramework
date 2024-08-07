namespace LMS.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(length: 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 3)]
        public required string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(length: 6)]
        public string Password { get; set; } = string.Empty;

        private Role Role { get; set; } = Role.Student;

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
        // TODO: Add RoleName?
        public string SearchableString => $"{FirstName} {LastName} {Email}";

        // Navigation properties
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
