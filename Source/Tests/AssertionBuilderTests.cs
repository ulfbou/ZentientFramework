using Zentient.Tests;


namespace Tests;

[TestClass]
public class AssertionBuilderTests
{
    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldPass_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Same");

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldFail_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Different");

        // Act & Assert
        Validate.Fail(() => Zentient.Tests.Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotEqualTo_ShouldPass_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new MyClass();
        var different = new MyClass();

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That(subject)
            .IsNotEqualTo(different));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotEqualTo_ShouldFail_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new MyClass();
        var same = subject;

        // Act & Assert
        Validate.Fail(() => Zentient.Tests.Assert
            .That(subject)
            .IsNotEqualTo(same));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldPass_WhenObjectsAreSameInstance()
    {
        // Arrange
        var subject = new object();
        var expected = subject;

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldFail_WhenObjectsAreNotSameInstance()
    {
        // Arrange
        var subject = new object();
        var expected = new object();

        // Act & Assert
        Validate.Fail(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldPass_WhenObjectIsNull()
    {
        // Arrange
        object subject = null;

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldFail_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new object();

        // Act & Assert
        Validate.Fail(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldPass_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new object();

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldFail_WhenObjectIsNull()
    {
        // Arrange
        object subject = null;

        // Act & Assert
        Validate.Fail(() => Zentient.Tests.Assert
            .That<object>(subject)
            .IsNotNull());
    }
}

internal class MyClass
{
    public MyClass()
    {
    }
}