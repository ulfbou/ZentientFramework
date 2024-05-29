using Zentient.Tests;
using Zentient.Configurator;
using Zentient.Configurator.Schemas;
using FluentValidation;

// Generate test methods for the Configuration class.
namespace Tests
{
    [TestClass]
    public class ConfigurationSchemaTests
    {

        public Assert Assert { get => Assert.Instance; }
        public string SchemaName => "SampleSchema";
        public string PropertyName1 => "Property1";
        public string PropertyName2 => "Property2";
        public string PropertyName3 => "Property3";
        public string PropertyType1 => "string";
        public string PropertyType2 => "int";
        public string PropertyType3 => "bool";

        public object ValidStringValue => "ValidValue";
        public object ValidIntValue => 123;
        public object InvalidStringValue => "InvalidValue";
        public object invalidIntValue => 124;
        public string InvalidValue => "Invalid value";
        public string TestId => "test-id";
        public string ExtraProperty => "Extra Property";
        public object ExtraValue => "Extra Value";

        [TestMethod]
        public void TryValidate_WhenCalledWithValidConfiguration_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Pass(result);
            Assert.Pass(validationResult.IsValid);
            Assert.Pass(validationResult.Errors.Count == 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithValidConfiguration_NotRequiredProperty_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2, false)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Pass(result);
            Assert.Pass(validationResult.IsValid);
            Assert.Pass(validationResult.Errors.Count == 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidIntValue },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, ValidIntValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_NotRequiredProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2, false)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidIntValue },
                    { PropertyName3, ValidIntValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_InvalidPropertyType_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, InvalidStringValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_InvalidPropertyType_NotRequiredProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2, false)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, InvalidStringValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_InvalidPropertyType_InvalidValue_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, invalidIntValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_InvalidPropertyType_InvalidValue_NotRequiredProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2, false)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, invalidIntValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithInvalidConfiguration_InvalidPropertyType_InvalidValue_InvalidValue_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, invalidIntValue },
                    { PropertyName3, InvalidValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        // Generate tests to cover missing required properties, extra properties without definition in a schema, case sensitivity, and other edge cases.
        [TestMethod]
        public void TryValidate_WhenCalledWithMissingRequiredProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithExtraProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, ExtraValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithExtraProperty_NotRequiredProperty_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2, false)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName3, true },
                    { ExtraProperty, ExtraValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Pass(result);
            Assert.Pass(validationResult.IsValid);
            Assert.Pass(validationResult.Errors.Count == 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithExtraProperty_ExtraPropertyType_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, 123 }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        // case sensitivity tests
        [TestMethod]
        public void TryValidate_WhenCalledWithCaseSensitivePropertyNames_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty("property1", PropertyType1)
                .WithProperty("property2", PropertyType2)
                .WithProperty("property3", PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { "Property1", ValidStringValue },
                    { "Property2", ValidIntValue },
                    { "Property3", true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithCaseSensitivePropertyNames_NotRequiredProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty("property1", PropertyType1)
                .WithProperty("property2", PropertyType2, false)
                .WithProperty("property3", PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { "Property1", ValidStringValue },
                    { "Property3", true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithCaseSensitivePropertyNames_ExtraProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty("property1", PropertyType1)
                .WithProperty("property2", PropertyType2)
                .WithProperty("property3", PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { "Property1", ValidStringValue },
                    { "Property2", ValidIntValue },
                    { "Property3", true },
                    { ExtraProperty, ExtraValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result);
            Assert.Fail(validationResult.IsValid);
            Assert.Fail(validationResult.Errors.Count > 0);
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithNullValue_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, null! },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result, "Null value should fail validation.");
            Assert.Fail(validationResult.IsValid, "Null value should fail validation.");
            Assert.Fail(validationResult.Errors.Count > 0, "Null value should emit validation errors.");
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithoutOptionalValue_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1, false)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Act & Assert
            Assert.Pass(result, "Null value should pass validation.");
            Assert.Pass(validationResult.IsValid, "Null value should pass validation.");
            Assert.Pass(validationResult.Errors.Count == 0, "Null value should not emit validation errors.");
        }

        // separate act and assert since TryValidate doesnt throw any exceptions
        [TestMethod]
        public void TryValidate_WhenCalledWithNullValue_ExtraProperty_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName1, ValidStringValue },
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, null! }
                }
            };

            // Act 
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result, "Null value should fail validation.");
            Assert.Fail(validationResult.IsValid, "Null value should fail validation.");
            Assert.Fail(validationResult.Errors.Count > 0, "Null value should emit validation errors.");
        }


        // What is the expected behavior when a configuration contains a property that is not part of the schema?

        [TestMethod]
        public void TryValidate_WhenCalledWithExtraProperty_NotDefinedInSchema_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1, false)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, ValidStringValue }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Pass(result, "Extra property should pass validation.");
            Assert.Pass(validationResult.IsValid, "Extra property should pass validation.");
            Assert.Pass(validationResult.Errors.Count == 0, "Extra property should not emit validation errors.");
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithNullValue_ExtraProperty_NotRequiredProperty_ShouldPass()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1, false)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, null! }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Pass(result, "Null value should pass validation.");
        }

        [TestMethod]
        public void TryValidate_WhenCalledWithExtraProperty_NotDefinedInSchema_ExtraPropertyType_ShouldFail()
        {
            // Arrange
            var schema = new ConfigurationSchemaBuilder()
                .WithName(SchemaName)
                .WithProperty(PropertyName1, PropertyType1, false)
                .WithProperty(PropertyName2, PropertyType2)
                .WithProperty(PropertyName3, PropertyType3)
                .Build();

            var configuration = new Configuration
            {
                SchemaName = SchemaName,
                Data = new Dictionary<string, object>
                {
                    { PropertyName2, ValidIntValue },
                    { PropertyName3, true },
                    { ExtraProperty, 123 }
                }
            };

            // Act
            var result = schema.TryValidate(configuration, out var validationResult);

            // Assert
            Assert.Fail(result, "Extra property with invalid type should fail validation.");
        }
    }
}
