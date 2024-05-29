using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zentient.Tests;
using Zentient.Configurator;
using Zentient.Configurator.Contexts;
using Zentient.Configurator.Repositories;
using Zentient.Configurator.Services;
using Zentient.Configurator.Schemas;
using System.Collections.Generic;

// Generate test methods for the Configuration class.
namespace Tests
{
    public class ConfiguratorTests
    {
        public Assert Assert { get => Assert.Instance; }

        public static string Id => "TestConfig";
        public static string SchemaName => "TestValue";
        public static string TestKey => "TestKey";
        public static string TestValue => "TestValue";
        public static Dictionary<string, object> Data = new Dictionary<string, object>();

        // Test methods to validate the Configuration class.
        // Generate signatures for test methods to validate the functionality of Zentient.Configurator.
        [TestMethod]
        public void Validate_TestConfig_Configuration()
        {
            // Arrange

            var configuration = new Configuration
            {
                Id = Id,
                SchemaName = SchemaName,
                Data = Data
            };

            // Act & Assert
            Assert.Pass(configuration.Id.Equals(Id),
                @$"Id Should be equal to ""{Id}"".");

            Assert.Pass(configuration.SchemaName.Equals(SchemaName),
                @$"SchemaName should be equal to ""{SchemaName}"".");

            Assert.Pass(configuration.Data[TestKey].Equals(TestValue),
                @$"Data should contain key ""{TestKey}"" with value ""{TestValue}"".");
        }

        [TestMethod]
        public void SetProperty_WhenCalled_SetsPropertyInDataDictionary()
        {
            // Arrange
            var configuration = new Configuration();
            var key = "TestKey";
            var value = "TestValue";

            // Act
            configuration.SetProperty(key, value);

            // Assert
            Assert.Pass(configuration.Data.ContainsKey(key));
            Assert.Pass(configuration.Data[key].Equals(value));
        }

        [TestMethod]
        public void SetProperty_WhenCalledWithExistingKey_UpdatesValueInDataDictionary()
        {
            // Arrange
            var configuration = new Configuration();
            var key = "TestKey";
            var oldValue = "OldValue";
            var newValue = "NewValue";

            configuration.SetProperty(key, oldValue);

            // Act
            configuration.SetProperty(key, newValue);

            // Assert
            Assert.Pass(configuration.Data.ContainsKey(key));
            Assert.Pass(configuration.Data[key].Equals(newValue));
        }

        [TestMethod]
        public void SetProperty_WhenCalledWithNullData_InitializesDataAndSetsProperty()
        {
            // Arrange
            var configuration = new Configuration { Data = null };
            var key = "TestKey";
            var value = "TestValue";

            // Act
            configuration.SetProperty(key, value);

            // Assert
            Assert.Pass(configuration.Data is not null);
            Assert.Pass(configuration.Data!.ContainsKey(key));
            Assert.Pass(configuration.Data[key].Equals(value));
        }
    }
}
