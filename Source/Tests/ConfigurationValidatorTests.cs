using Zentient.Tests;
using Zentient.Configurator;
using Zentient.Configurator.Schemas;
using Moq;
using Zentient.Configurator.Validation;
using FluentValidation.Results;

// Generate test methods for the Configuration class.
namespace Tests
{
    public class ConfigurationValidatorTests
    {
        [TestMethod]
        public void Validate_ValidSchema_ReturnsTrue()
        {
            // Arrange
            var mockSchema = new Mock<ConfigurationSchema>();
            ValidationResult validationResult;
            mockSchema.Setup(s =>
                s.TryValidate(
                    It.IsAny<Configuration>(),
                    out validationResult)).Returns(true);

            var validator = new ConfigurationValidator();
            var config = new Configuration();

            // Act
            var result = validator.Validate(mockSchema.Object, config);

            // Assert
            Assert.Pass(result);
        }

        [TestMethod]
        public void Validate_InvalidSchema_ReturnsFalse()
        {
            // Arrange
            var mockSchema = new Mock<ConfigurationSchema>();
            ValidationResult configuration;
            mockSchema.Setup(s =>
                s.TryValidate(
                    It.IsAny<Configuration>(),
                    out configuration)).Returns(false);

            var validator = new ConfigurationValidator();
            var config = new Configuration();

            // Act
            var result = validator.Validate(mockSchema.Object, config);

            // Assert
            Assert.Fail(result);
        }
    }
}
