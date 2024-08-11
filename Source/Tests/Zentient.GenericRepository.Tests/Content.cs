using Zentient.Core;

namespace Zentient.GenericRepository.Tests
{
    public class ContentDTO : IEntity, INamedEntity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string GetIdentifier() => Name;
    }

    public class Content : IEntity, INamedEntity, ICategoryEntity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ContentId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public User Author { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string GetIdentifier() => ContentId;
    }
}