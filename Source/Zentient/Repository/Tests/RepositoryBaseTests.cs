using Microsoft.EntityFrameworkCore;
using Zentient.Repository.Tests.Data;
using Zentient.Repository.Tests.Models;

namespace Zentient.Repository.Tests
{
    [TestClass]
    public class RepositoryBaseTests
    {
        private SampleDbContext _context;
        private RepositoryBase<SampleEntity, int> _repository;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<SampleDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SampleDbContext(options);
            _repository = new RepositoryBase<SampleEntity, int>(_context);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Test Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Entity", result.Name);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task FindAsync_ShouldReturnMatchingEntities()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(e => e.Name.Contains("1"));

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "New Entity" };

            // Act
            var result = await _repository.AddAsync(entity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, await _context.SampleEntities.CountAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            entity.Name = "Updated Entity";

            // Act
            var result = await _repository.UpdateAsync(entity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Entity", (await _context.SampleEntities.FindAsync(1))?.Name);
        }

        [TestMethod]
        public async Task RemoveAsync_ShouldRemoveEntity()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.RemoveAsync(entity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, await _context.SampleEntities.CountAsync());
        }

        // Generate code for positive tests for the following methods:
        // GetPagedAsync
        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnPagedEntities()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(1, 2);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.TotalPages);
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnPagedEntitiesWithFilter()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(1, 2, e => e.Name.Contains("3"));

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnPagedEntitiesWithOrderBy()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(1, 2, orderBy: e => e.OrderBy(entity => entity.Name));

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual("Entity 1", result[0].Name);
            Assert.AreEqual("Entity 2", result[1].Name);
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnPagedEntitiesWithOrderByDescending()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(1, 2, orderBy: e => e.OrderByDescending(entity => entity.Name));

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual("Entity 5", result[0].Name);
            Assert.AreEqual("Entity 4", result[1].Name);
        }

        // GetPagedByCursorAsync
        [TestMethod]
        public async Task GetPagedByCursorAsync_ShouldReturnPagedEntities()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedByCursorAsync(0, 2, orderBy: e => e.OrderBy(entity => entity.Id));

            // Assert
            Assert.AreEqual(2, result.Count());

        }

        [TestMethod]
        public async Task GetPagedByCursorAsync_ShouldReturnPagedEntitiesWithFilter()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedByCursorAsync(0, 2,
                orderBy: e => e.OrderBy(entity => entity.Id),
                filter: e => e.Name.Contains("3"));

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        public async Task GetPagedByCursorAsync_ShouldReturnPagedEntitiesWithOrderBy()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedByCursorAsync(0, 2, filter: e => e.Id < 3, orderBy: e => e.OrderBy(entity => entity.Name));

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.TotalPages);
            Assert.AreEqual("Entity 1", result[0].Name);
            Assert.AreEqual("Entity 2", result[1].Name);
        }

        [TestMethod]
        public async Task GetPagedByCursorAsync_ShouldReturnPagedEntitiesWithOrderByDescending()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" },
                new SampleEntity { Id = 3, Name = "Entity 3" },
                new SampleEntity { Id = 4, Name = "Entity 4" },
                new SampleEntity { Id = 5, Name = "Entity 5" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedByCursorAsync(0, 2, filter: e => e.Id > 3, orderBy: e => e.OrderByDescending(entity => entity.Name));

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.TotalPages);
            Assert.AreEqual("Entity 5", result[0].Name);
            Assert.AreEqual("Entity 4", result[1].Name);
        }

        // SoftDeleteAsync
        [TestMethod]
        public async Task SoftDeleteAsync_ShouldSoftDeleteEntity()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SoftDeleteAsync(entity);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Entity is ISoftDeletable<int> deletable && deletable.IsDeleted);
        }

        // SoftUndeleteAsync
        [TestMethod]
        public async Task SoftUndeleteAsync_ShouldSoftUnDeleteEntity()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SoftDeleteAsync(entity);
            var result2 = await _repository.SoftUndeleteAsync(entity);

            // Assert
            Assert.IsNotNull(result2);
            Assert.IsTrue(result2.Entity is ISoftDeletable<int> deletable && !deletable.IsDeleted);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Test Entity" };
            await _context.SampleEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAsync(2);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindAsync_ShouldReturnEmpty_WhenNoEntitiesMatchPredicate()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(e => e.Name.Contains("3"));

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnEmpty_WhenNoEntitiesMatchFilter()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(1, 2, e => e.Name.Contains("3"));

            // Assert
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        public async Task GetPagedByCursorAsync_ShouldReturnEmpty_WhenNoEntitiesMatchFilter()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity 1" },
                new SampleEntity { Id = 2, Name = "Entity 2" }
            };
            await _context.SampleEntities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedByCursorAsync(0, 2, filter: e => e.Name.Contains("3"));

            // Assert
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(0, result.TotalPages);
        }
    }
}
