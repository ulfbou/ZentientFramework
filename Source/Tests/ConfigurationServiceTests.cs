using MongoDB.Driver;
using Moq;
using Zentient.Configurator.Contexts;
using Zentient.Configurator.Services;
using Zentient.Configurator.Validation;
using Zentient.Configurator.Schemas;
using Zentient.Tests;
using Zentient.Configurator;
using System.ComponentModel.DataAnnotations;

// Generate test methods for the Configuration class.
namespace Tests
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        public Assert Assert { get => Assert.Instance; }

        public string SchemaName => "SampleSchema";
        public string Property1 => "Property1";
        public string Property2 => "Property2";
        public string Value1 => "string";
        public string Value2 => "int";
        public string InvalidValue => "Invalid value";
        public string TestId => "test-id";

        [TestMethod]
        public async Task CreateSchema_SavesSchemaCorrectly()
        {
            // Arrange
            var context = new Mock<ConfigurationContext>("mongodb://localhost:27017");
            var validator = new ConfigurationValidator();
            var service = new ConfigurationService(context.Object, validator);

            // Act
            await service.CreateSchema(builder =>
                builder.WithName(SchemaName)
                       .WithProperty(Property1, Value1)
                       .WithProperty(Property2, Value2));

            var schema = await service.GetSchema(SchemaName);

            // Assert
            Assert.That(schema.Name).IsEqualTo(SchemaName);
            Assert.That(schema.Properties).IsNotEqualTo(null!);
            Assert.That(schema.Properties[Property1].Type).IsEqualTo(Value1);
            Assert.That(schema.Properties[Property2].Type).IsEqualTo(Value2);
        }

        [TestMethod]
        public async Task CreateConfiguration_ValidConfiguration_SavesConfiguration()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<Configuration>>();
            var context = new Mock<ConfigurationContext>("mongodb://localhost:27017");
            context.Setup(c => c.Configurations).Returns(mockCollection.Object);

            var validator = new Mock<ConfigurationValidator>();
            validator.Setup(v => v.Validate(It.IsAny<Schema>(), It.IsAny<Configuration>())).Returns(true);

            var service = new ConfigurationService(context.Object, validator.Object);

            // Act
            await service.CreateConfiguration(builder =>
                builder.WithSchema(SchemaName)
                       .WithProperty(Property1, Value1)
                       .WithProperty(Property2, Value2));
            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { Property1, Value1 },
                    { Property2, Value2 }
                }
            };

            // Assert
            mockCollection.Verify(c => c.InsertOne(It.IsAny<Configuration>(), null, default), Times.Once);
        }

        [TestMethod]
        public void CreateConfiguration_InvalidConfiguration_ThrowsValidationException()
        {
            // Arrange
            var context = new Mock<ConfigurationContext>("mongodb://localhost:27017");
            var validator = new Mock<ConfigurationValidator>();
            validator.Setup(v => v.Validate(It.IsAny<Schema>(), It.IsAny<Configuration>())).Returns(false);

            var service = new ConfigurationService(context.Object, validator.Object);

            // Act & Assert
            Assert.That(async () =>
                await service.CreateConfiguration(builder =>
                    builder.WithSchema(SchemaName)
                           .WithProperty(Property1, InvalidValue)))
                .Throws<ValidationException>();
        }

        [TestMethod]
        public async Task ApplyConfigurationTo_ConfigurableObject_AppliesConfiguration()
        {
            // Arrange
            var mockConfigurable = new Mock<IConfigurable>();
            var config = new Configuration
            {
                Id = TestId,
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { Property1, Value1 },
                    { Property2, Value2 }
                }
            };

            var mockCollection = new Mock<IMongoCollection<Configuration>>();
            mockCollection
                .Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Configuration>>(), It.IsAny<FindOptions<Configuration, Configuration>>(), default(CancellationToken)))
                .Returns(Task.FromResult((IAsyncCursor<Configuration>)new List<Configuration>(new List<Configuration> { config })));

            var context = new Mock<ConfigurationContext>("mongodb://localhost:27017");
            context.Setup(c => c.Configurations).Returns(mockCollection.Object);

            var validator = new ConfigurationValidator();
            var service = new ConfigurationService(context.Object, validator);

            // Act
            await service.ApplyConfigurationTo(mockConfigurable.Object, "test-id");

            // Assert
            mockConfigurable.Verify(c => c.ApplyConfiguration(It.Is<Configuration>(config => config.Id == "test-id")), Times.Once);
        }
    }
}
