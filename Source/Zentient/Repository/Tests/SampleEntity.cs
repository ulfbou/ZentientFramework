﻿namespace Zentient.Repository.Tests
{
    public class SampleEntity : ISoftDeletable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
    }
}
