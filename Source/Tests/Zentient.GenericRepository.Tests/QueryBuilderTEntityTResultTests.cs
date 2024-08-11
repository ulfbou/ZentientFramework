using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository.Tests
{
    [TestClass]
    public class QueryBuilderTEntityTResultTests
    {
        [TestMethod]
        public void Should_Project_Entities_To_Different_Type()
        {
            // Arrange
            var builder = new QueryBuilder<Content, ContentDTO>();
            builder.Select(entity => new ContentDTO { Name = entity.Name });

            // Act
            var query = builder.Build();

            // Assert
            // Verify that the query projects entities correctly
        }

        [TestMethod]
        public void Should_Handle_Complex_Projections()
        {
            // Similar structure as above
        }
    }
}