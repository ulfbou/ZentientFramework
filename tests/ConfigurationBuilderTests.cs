// <copyright file="ConfigurationBuilderTests.cs" company="Zentient">
// Copyright © 2025 Zentient. All rights reserved.
// </copyright>

using Xunit;
using FluentAssertions;

namespace Zentient.NewLibrary.Tests;

/// <summary>Tests for <see cref="ConfigurationBuilder"/> class.</summary>
[Trait("Category", "Unit")]
[Collection("ConfigurationBuilder Collection")]
public class ConfigurationBuilderTests
{
    [Fact(DisplayName = "Constructor initializes with empty values")]
    [Trait("Scenario", "Initialization")]
    public void Constructor_ShouldInitializeWithEmptyValues()
    {
        // Act
        var builder = new ConfigurationBuilder();

        // Assert
        builder.Name.Should().Be(string.Empty);
        builder.Description.Should().Be(string.Empty);
        builder.Metadata.Should().BeEmpty();
    }

    [Theory(DisplayName = "WithName sets the Name property")]
    [InlineData("Test Configuration")]
    [InlineData("Another Name")]
    [Trait("Scenario", "Setters")]
    public void WithName_ShouldSetName(string expectedName)
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act
        var result = builder.WithName(expectedName);

        // Assert
        result.Should().BeSameAs(builder);
        builder.Name.Should().Be(expectedName);
    }

    [Theory(DisplayName = "WithDescription sets the Description property")]
    [InlineData("Test description")]
    [InlineData("Another description")]
    [Trait("Scenario", "Setters")]
    public void WithDescription_ShouldSetDescription(string expectedDescription)
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act
        var result = builder.WithDescription(expectedDescription);

        // Assert
        result.Should().BeSameAs(builder);
        builder.Description.Should().Be(expectedDescription);
    }

    [Fact(DisplayName = "Fluent configuration works correctly")]
    [Trait("Scenario", "Fluent API")]
    public void FluentConfiguration_ShouldWorkCorrectly()
    {
        // Arrange
        const string expectedName = "Fluent Configuration";
        const string expectedDescription = "Built using fluent API";

        // Act
        var builder = new ConfigurationBuilder()
            .WithName(expectedName)
            .WithDescription(expectedDescription);

        // Assert
        builder.Name.Should().Be(expectedName);
        builder.Description.Should().Be(expectedDescription);
    }

    [Theory(DisplayName = "WithMetadata adds key-value pairs")]
    [InlineData("environment", "development")]
    [InlineData("region", "us-east")]
    [Trait("Scenario", "Metadata")]
    public void WithMetadata_ShouldAddMetadataKeyValue(string key, string value)
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act
        var result = builder.WithMetadata(key, value);

        // Assert
        result.Should().BeSameAs(builder); // Fluent interface
        builder.Metadata.Should().ContainKey(key);
        builder.Metadata[key].Should().Be(value);
    }

    [Fact(DisplayName = "IsValid returns false when name or description missing")]
    [Trait("Scenario", "Validation")]
    public void IsValid_ShouldReturnFalse_WhenNameOrDescriptionMissing()
    {
        // Arrange & Act & Assert
        new ConfigurationBuilder().IsValid().Should().BeFalse(); // Empty
        new ConfigurationBuilder().WithName("Test").IsValid().Should().BeFalse(); // Missing description
        new ConfigurationBuilder().WithDescription("Test").IsValid().Should().BeFalse(); // Missing name
    }

    [Fact(DisplayName = "IsValid returns true when name and description set")]
    [Trait("Scenario", "Validation")]
    public void IsValid_ShouldReturnTrue_WhenNameAndDescriptionSet()
    {
        // Arrange
        var builder = new ConfigurationBuilder()
            .WithName("TestConfig")
            .WithDescription("Test configuration");

        // Act & Assert
        builder.IsValid().Should().BeTrue();
    }

    [Fact(DisplayName = "GetScope returns null when scope not set")]
    [Trait("Scenario", "Metadata")]
    public void GetScope_ShouldReturnNull_WhenScopeNotSet()
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act & Assert
        builder.GetScope().Should().BeNull();
    }

    [Theory(DisplayName = "GetScope returns scope when set in metadata")]
    [InlineData("application")]
    [InlineData("user")]
    [Trait("Scenario", "Metadata")]
    public void GetScope_ShouldReturnScope_WhenScopeSetInMetadata(string expectedScope)
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act
        builder.WithMetadata("scope", expectedScope);

        // Assert
        builder.GetScope().Should().Be(expectedScope);
    }
}
