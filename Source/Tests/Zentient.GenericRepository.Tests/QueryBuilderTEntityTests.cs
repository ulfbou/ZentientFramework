using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository.Tests
{
    [TestClass]
    public class QueryBuilderTEntityTests
    {
        private const string StoredContentName2 = "MSTest 101";
        private const string StoredContentName3 = "Test 101";
        private const string StoredContentName1 = "XUnit 101";
        private const string OrderedContentName1 = StoredContentName2;
        private const string OrderedContentName2 = StoredContentName3;
        private const string OrderedContentName3 = StoredContentName1;
        private const string StoredContentId1 = "1";
        private const string StoredContentId2 = "2";
        private const string StoredContentId3 = "3";
        private const string StoredCategory1 = ".NET";
        private const string StoredCategory2 = "C#";
        private const string StoredCategory3 = StoredCategory2;
        private const string OrderedCategory1 = StoredCategory2;
        private const string OrderedCategory2 = StoredCategory2;
        private const string OrderedCategory3 = StoredCategory1;
        private const string StoredAuthorName1 = "Uffe";
        private const string StoredAuthorName2 = "Tuffe";
        private const string OrderedAuthorName1 = StoredAuthorName2;
        private const string OrderedAuthorName2 = StoredAuthorName1;

        private IQueryable<User>? _userQueryable;
        private IQueryable<Content>? _contentQueryable;

        [TestInitialize]
        public void Setup()
        {
            _userQueryable = new List<User>
            {
                new User { UserId = "1", Name = StoredAuthorName1 },
                new User { UserId = "2", Name = StoredAuthorName2 }
            }.AsQueryable();
            _contentQueryable = new List<Content>
            {
                new Content { ContentId = StoredContentId1, Name = StoredContentName1, Category = StoredCategory1, Author = _userQueryable.First() },
                new Content { ContentId = StoredContentId2, Name = StoredContentName2, Category = StoredCategory2, Author = _userQueryable.Last() },
                new Content { ContentId = StoredContentId3, Name = StoredContentName3, Category = StoredCategory3, Author = _userQueryable.First() }
            }.AsQueryable();
        }

        [TestMethod]
        public async Task Should_Return_Correct_Count()
        {
            // Arrange
            var query = TestQueryObjects.GetQuery<Content>();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Should_Build_Simple_Where_Query()
        {
            // Arrange & Act
            var query = TestQueryObjects.GetEntityByIdQuery<Content>();

            // Assert
            var result = await query.ApplyAsync(_contentQueryable!, StoredContentId1);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(StoredContentName1, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetOrderedBy<Content>();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(OrderedContentName1, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Descending_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetOrderedBy<Content>(true);

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(OrderedContentName3, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_Take_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetTakeQuery<Content>(2);

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(StoredContentName1, result.First().Name);
            Assert.AreEqual(StoredContentName2, result.Take(1).First().Name);
        }

        [TestMethod]
        public async Task Should_Build_Skip_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetSkipQuery<Content>(1);

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Should_Build_Skip_Take_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetSkipTakeQuery<Content>(1, 1);

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(StoredContentName2, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Skip_Take_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetSkipTakeQuery<Content>(1, 1);
            var builder = new QueryBuilder<Content>();
            builder.OrderBy(entity => entity.Name);

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(StoredContentName2, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_GroupBy_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetGroupByCategoryQuery<Content>();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(OrderedCategory1, result.First().Category);
            Assert.AreEqual(OrderedCategory2, result.Skip(1).First().Category);
            Assert.AreEqual(OrderedCategory3, result.Skip(2).First().Category);
        }

        [TestMethod]
        public async Task Should_Build_Include_Query()
        {
            // Arrange
            var query = TestQueryObjects.GetIncludeQuery<Content>();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.IsNotNull(result.First().Author);
        }
    }
}