using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository.Tests
{
    public class QueryTEntityTests
    {
        List<Content> _content = new List<Content>
        {
            new Content { ContentId = "1", Name ="One", Category = "Desc. 1"},
            new Content { ContentId = "2", Name ="Two", Category = "Desc. 2"},
            new Content { ContentId = "3", Name ="Three", Category = "Desc. 3"},
            new Content { ContentId = "4", Name ="Four", Category = "Desc. 4"},
            new Content { ContentId = "5", Name ="Five", Category = "Desc. 5"}
        };

        List<User> _users = new List<User>
        {
            new User { UserId = Guid.NewGuid().ToString() }
        };

        QueryTemplates<Content> Templates { get; set; } = new();

        [TestMethod]
        public async Task Should_Execute_Query_With_Parameters()
        {
            // Arrange
            var cancellation = CancellationToken.None;
            var mockRepository = new Mock<IGenericRepository<Content>>();
            mockRepository
                .Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string id, CancellationToken token) => new List<Content>
                    {
                        new Content { ContentId = "1" },
                        new Content { ContentId = "2" },
                        new Content { ContentId = "3" }
                    }.Where(c => c.ContentId == id));

            var handler = Templates.GetByIdAsync();
            var query = new Query<Content>(handler);
            var entities = _content;

            var queryable = entities.AsQueryable();

            // Act
            var result = await query.ApplyAsync(queryable, "3");

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task Should_Execute_Query_With_Include()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Content>>();
            var handler = Templates.GetByIdAsync<User>();
            var query = new Query<Content>(handler);
            var entities = new List<Content>
                {
                    new Content { ContentId = "1", Author = new User { UserId = "1" } },
                    new Content { ContentId = "2", Author = new User { UserId = "2" } },
                    new Content { ContentId = "3", Author = new User { UserId = "3" } }
                };

            var queryable = entities.AsQueryable();

            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((string id, CancellationToken token) => entities.Where(c => c.ContentId == id));

            // Act
            var result = await query.ApplyAsync(queryable, "3", c => c.Author);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsNotNull(result.First().Author);
        }



        // Add more test methods for different scenarios

        //Query execution:
        //Should_Execute_Query_With_Parameters
        //Should_Execute_Query_With_Include
        //Should_Handle_Invalid_Parameters
        //Performance:
        //Should_Execute_Query_Within_Time_Limit
        //Error handling:
        //Should_Throw_Exception_For_Invalid_Parameters
    }
}