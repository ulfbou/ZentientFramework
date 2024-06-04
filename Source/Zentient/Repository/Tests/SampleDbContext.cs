using Microsoft.EntityFrameworkCore;
using Zentient.Repository.Tests.Models;

namespace Zentient.Repository.Tests.Data
{

    public class SampleDbContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }
    }
}
