using Zentient.Tests;
using Zentient.Configurator.Schemas;

// Generate test methods for the Configuration class.
namespace Tests
{
    [TestClass]
    public class ConfigurationSchemaBuilderTests
    {
        public Assert Assert { get => Assert.Instance; }

        [TestMethod]
        public void Build_SchemaWithName()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();
            var schemaName = "TestSchema";

            // Act
            var schema = builder.WithName(schemaName).Build();

            // Assert
            Assert.That(schema.Name).IsEqualTo(schemaName);
        }

        [TestMethod]
        public void Build_SchemaWithProperties()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();
            var schemaName = "TestSchema";
            var key1 = "Key1";
            var key2 = "Key2";
            var value1 = "string";
            var value2 = "int";

            // Act
            var schema = builder.WithName(schemaName)
                                .WithProperty(key1, value1)
                                .WithProperty(key2, value2)
                                .Build();

            // Assert
            Assert.That(schema.Name).IsEqualTo(schemaName);
            Assert.That(schema.Properties).IsNotNull();
            Assert.That(schema.Properties[key1].Type).IsEqualTo(value1);
            Assert.That(schema.Properties[key2].Type).IsEqualTo(value2);
        }

        [TestMethod]
        public void WithName_WhenCalledWithEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithName(""))
                .ThrowsExactly<ArgumentException>("WithName called with an empty string.");
        }

        [TestMethod]
        public void WithName_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithName(null!))
                .ThrowsExactly<ArgumentNullException>("WithName called with null.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithEmptyKey_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("", "string"))
                .ThrowsExactly<ArgumentException>("WithProperty called with an empty key.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty(null!, "string"))
                .ThrowsExactly<ArgumentNullException>("WithProperty called with null key.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithEmptyType_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("key", ""))
                .ThrowsExactly<ArgumentException>("WithProperty called with an empty type.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithNullType_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationSchemaBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("key", null!))
                .ThrowsExactly<ArgumentNullException>("WithProperty called with null type.");
        }
    }
}
