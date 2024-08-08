namespace LMS.Core.Entities
{
    public class User : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Submission> Submissions { get; set; }
        public ICollection<Thread> Threads { get; set; }
    }
}