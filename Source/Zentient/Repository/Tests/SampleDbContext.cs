using Microsoft.EntityFrameworkCore;

namespace Zentient.Repository.Tests
{

    public class SampleDbContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }
    }
}
