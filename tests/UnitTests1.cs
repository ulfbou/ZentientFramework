// <copyright file="PackageServiceTests.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Xunit;

using Zentient.Template;

namespace Zentient.Template.Tests
{
    [Trait("Category", "Unit")]
    public class PackageServiceTests
    {
        [Fact(DisplayName = "GetGreeting returns correct message when package name is provided")]
        [Trait("Method", nameof(PackageService.GetGreeting))]
        public void GetGreeting_WithProvidedName_ShouldReturnCorrectMessage()
        {
            var testPackageName = "MyCustomPackage";
            var service = new PackageService(testPackageName);
            var expected = $"Hello from {testPackageName}!";

            var actual = service.GetGreeting();

            actual.Should().Be(expected);
        }

        [Fact(DisplayName = "GetGreeting returns assembly name greeting when no name is provided")]
        [Trait("Method", nameof(PackageService.GetGreeting))]
        public void GetGreeting_WithoutProvidedName_ShouldReturnAssemblyGreeting()
        {
            var service = new PackageService();
            var expectedPackageName = typeof(PackageService).Assembly.GetName().Name;
            var expected = $"Hello from {expectedPackageName}!";

            var actual = service.GetGreeting();

            actual.Should().Be(expected);
        }

        [Fact(DisplayName = "PackageName property reflects constructor argument")]
        [Trait("Method", nameof(PackageService.PackageName))]
        public void PackageName_Property_ShouldReflectConstructorArgument()
        {
            var expectedName = "AnotherTestPackage";
            var service = new PackageService(expectedName);

            var actualName = service.PackageName;

            actualName.Should().Be(expectedName);
        }

        [Fact(DisplayName = "PackageName property defaults to assembly name")]
        [Trait("Method", nameof(PackageService.PackageName))]
        public void PackageName_Property_ShouldDefaultToAssemblyName()
        {
            var service = new PackageService();
            var expectedDefaultName = typeof(PackageService).Assembly.GetName().Name;

            var actualDefaultName = service.PackageName;

            actualDefaultName.Should().Be(expectedDefaultName);
        }

        [Theory(DisplayName = "ProcessInput transforms the string correctly")]
        [InlineData("data", "Processed: data by ")]
        [InlineData("test string", "Processed: test string by ")]
        [Trait("Method", nameof(PackageService.ProcessInput))]
        public void ProcessInput_ShouldTransformString(string input, string expectedPrefix)
        {
            var testPackageName = "Processor";
            var service = new PackageService(testPackageName);
            var expected = $"{expectedPrefix}{testPackageName}";

            var actual = service.ProcessInput(input);

            actual.Should().Be(expected);
        }
    }
}
