using Zentient.Tests;
using Zentient.Configurator;

// Generate test methods for the Configuration class.
namespace Tests
{
    [TestClass]
    public class ConfigurationBuilderTests
    {
        Assert Assert => Assert.Instance;

        [TestMethod]
        public void Build_ConfigurationWithSchemaName()
        {
            // Arrange
            var builder = new ConfigurationBuilder();
            var schemaName = "TestSchema";

            // Act
            var config = builder.WithSchema(schemaName).Build();

            // Assert
            Assert.That(config.SchemaName).IsEqualTo(schemaName);
        }

        [TestMethod]
        public void Build_ConfigurationWithProperties()
        {
            // Arrange
            var builder = new ConfigurationBuilder();
            var schemaName = "TestSchema";
            var key1 = "Key1";
            var value1 = "Value1";
            var key2 = "Key";
            var value2 = "Value2";

            // Act
            var config = builder.WithSchema(schemaName)
                                .WithProperty(key1, value1)
                                .WithProperty(key2, value2)
                                .Build();

            // Assert
            Assert.That(config.SchemaName).IsEqualTo(schemaName);
            Assert.That(config.Data).IsNotNull();
            Assert.That(config.Data[key1]).IsEqualTo(value1);
            Assert.That(config.Data[key2]).IsEqualTo(value2);
        }

        [TestMethod]
        public void WithSchema_WhenCalledWithEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            Assert.That(() => builder.WithSchema(""))
                .ThrowsExactly<ArgumentException>("WithSchema called with an empty string.");
        }

        [TestMethod]
        public void WithSchema_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act
            Assert.That(() => builder.WithSchema(null))
                .ThrowsExactly<ArgumentNullException>("WithSchema called with a null string.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithEmptyStringKey_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("", "value"))
                .ThrowsExactly<ArgumentException>("WithProperty called with an empty key.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithProperty_WhenCalledWithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty(null!, "Value"))
                .ThrowsExactly<ArgumentNullException>("WithProperty called with a null value.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithEmptyStringValue_ThrowsArgumentException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("key", ""))
                .ThrowsExactly<ArgumentException>("WithProperty called with an empty value.");
        }

        [TestMethod]
        public void WithProperty_WhenCalledWithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            Assert.That(() => builder.WithProperty("key", null!))
                .ThrowsExactly<ArgumentNullException>("WithProperty called with a null value.");
        }
    }
}
