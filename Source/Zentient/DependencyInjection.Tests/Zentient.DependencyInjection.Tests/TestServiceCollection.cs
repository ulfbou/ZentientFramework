using Zentient.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zentient.DependencyInjection.Tests
{
    [TestClass]
    public class TestServiceCollection
    {
        // Test methods for Zentient.DependencyInjection
        // IServiceCollection: 
        // AddSingleton
        // AddScoped
        // AddTransient

        [TestMethod]
        public void AddSingleton_Should_Be_Registered()
        {
            // Arrange & Act
            IServiceCollection services = new ServiceCollection();
            var provider = services.AddSingleton<ITestService, TestService>().Build();

            // Assert
            var service = provider.GetService<ITestService>();
            Assert.IsTrue(service != null);
        }

        // Test that the provider actually returns the same instance when registered as singleton
        [TestMethod]
        public void AddSingleton_Should_Return_Same_Instance()
        {
            // Arrange & Act
            IServiceCollection services = new ServiceCollection();
            var provider = services.AddSingleton<ITestService, TestService>().Build();

            // Assert
            var service1 = provider.GetService<ITestService>();
            var service2 = provider.GetService<ITestService>();
            Assert.IsTrue(service1 == service2);
        }

        [TestMethod]
        public void AddScoped_Should_Be_Registered()
        {
            // Arrange & Act
            IServiceCollection services = new ServiceCollection();
            var provider = services.AddScoped<ITestService, TestService>().Build();

            // Assert
            var service = provider.GetService<ITestService>();
            Assert.IsTrue(service != null);
        }

        // Test that the provider actually returns the same instance when registered as scoped
        [TestMethod]
        public void AddScoped_Should_Return_Same_Instance()
        {
            // Arrange & Act
            IServiceCollection services = new ServiceCollection();
            var provider = services.AddScoped<ITestService, TestService>().Build();
            using (var scope = provider.CreateScope())
            {
                // Assert
                var service1 = provider.GetService<ITestService>();
                var service2 = provider.GetService<ITestService>();
                Assert.IsTrue(service1 == service2);
            }
        }

    }
}