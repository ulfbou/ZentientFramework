using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository.Tests
{
    [TestClass]
    public class QueryBuilderTEntityTests2
    {
        private IQueryable<User>? _userQueryable;
        private IQueryable<Content>? _contentQueryable;

        private const string Content1 = "XUnit";
        private const string Content2 = "MSTest";
        private const string Content3 = "NTest";

        [TestInitialize]
        public void Setup()
        {
            _userQueryable = new List<User>
            {
                new User { UserId = "1", Name = "Uffe" },
                new User { UserId = "2", Name = "Tuffe" }
            }.AsQueryable();
            _contentQueryable = new List<Content>
            {
                new Content { ContentId = "1", Name = Content1, Category = ".NET", Author = _userQueryable.First() },
                new Content { ContentId = "2", Name = Content2, Category = "C#", Author = _userQueryable.Last() },
                new Content { ContentId = "3", Name = Content3, Category = "C#", Author = _userQueryable.First() }
            }.AsQueryable();
        }

        [TestMethod]
        public async Task Should_Return_Correct_Count()
        {
            // Arrange
            var query = Query.From<Content>().Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Should_Build_Simple_Where_Query()
        {
            // Arrange & Act
            var query = Query.Where<Content>(
                (entity, parameters) => entity.GetIdentifier().Equals(parameters.Id))
                .Build();

            // Assert
            var result = await query.ApplyAsync(_contentQueryable!, "1");
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Content1, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Query()
        {
            // Arrange & Act
            var query = Query.OrderBy<Content>(entity => entity.Name)
                             .Build();

            // Assert
            var result = await query.ApplyAsync(_contentQueryable!);
            Assert.AreEqual(Content2, result.First().Name);
            Assert.AreEqual(Content3, result.Skip(1).First().Name);
            Assert.AreEqual(Content1, result.Last().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Descending_Query()
        {
            // Arrange & Act
            var query = Query.OrderByDescending<Content>(entity => entity.Name).Build();

            // Assert
            var result = await query.ApplyAsync(_contentQueryable!);
            Assert.AreEqual(Content3, result.First().Name);
            Assert.AreEqual(Content1, result.Skip(1).First().Name);
            Assert.AreEqual(Content2, result.Last().Name);
        }

        [TestMethod]
        public async Task Should_Build_Take_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .Take(2)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(Content1, result.First().Name);
            Assert.AreEqual(Content2, result.Last().Name);
        }

        [TestMethod]
        public async Task Should_Build_Skip_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .Skip(1)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(Content2, result.First().Name);
            Assert.AreEqual(Content3, result.Last().Name);
        }

        [TestMethod]
        public async Task Should_Build_Skip_Take_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .Skip(1)
                .Take(1)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Content2, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_OrderBy_Skip_Take_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .OrderBy(entity => entity.Name)
                .Skip(1)
                .Take(1)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Content2, result.First().Name);
        }

        [TestMethod]
        public async Task Should_Build_GroupBy_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .GroupBy(entity => entity.Category)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Should_Build_Include_Query()
        {
            // Arrange
            var query = Query.From<Content>()
                .Include(entity => entity.Author)
                .Build();

            // Act
            var result = await query.ApplyAsync(_contentQueryable!);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsNotNull(result.First().Author);
        }
    }
}