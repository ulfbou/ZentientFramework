namespace LMS.Core.Entities
{
    public class Activity : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Document> Documents { get; set; }
        public Guid ModuleId { get; set; }
        public Module Module { get; set; }
    }
}