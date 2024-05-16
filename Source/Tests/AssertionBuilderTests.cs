using Zentient.Tests;

namespace Tests;

[TestClass]
public class AssertionBuilderTests
{
    public Assert Assert { get => Assert.Instance; }

    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldPass_WhenObjectsAreEqual()
    {
        // Arrange
        string subject = "Same".ToString();
        string expected = new String("Same");

        // Act & Assert
        Validate.Pass(() => Assert
            .That<string>(subject).IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldFail_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Different");

        // Act & Assert
        Validate.Fail(() => Assert
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
        Validate.Pass(() => Assert
            .That<MyClass>(subject)
            .IsNotEqualTo(different));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotEqualTo_ShouldFail_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new MyClass();
        var same = subject;

        // Act & Assert
        Validate.Fail(() => Assert
            .That<MyClass>(subject)
            .IsNotEqualTo(same));
    }

    [TestMethod]
    public void AssertThatMyClass_IsSameAs_ShouldPass_WhenObjectsAreSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = subject;

        // Act & Assert
        Validate.Pass(() => Assert
            .That<MyClass>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsSameAs_ShouldFail_WhenObjectsAreNotSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = new MyClass();

        Assert.That<ICollection<int>>(new List<int>());
        // Act & Assert
        Validate.Fail(() => Assert
            .That<object>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNull_ShouldPass_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Validate.Pass(() => Assert
            .That<MyClass>(subject!)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNull_ShouldFail_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Validate.Fail(() => Assert
            .That<MyClass>(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotNull_ShouldPass_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Validate.Pass(() => Assert
            .That<MyClass>(subject)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotNull_ShouldFail_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Validate.Fail(() => Assert
            .That<MyClass>(subject!)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatObject_IsEqualTo_ShouldPass_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Same");

        // Act & Assert
        Validate.Pass(() => Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsEqualTo_ShouldFail_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Different");

        // Act & Assert
        Validate.Fail(() => Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsNotEqualTo_ShouldPass_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new MyClass();
        var different = new MyClass();

        // Act & Assert
        Validate.Pass(() => Assert
            .That(subject)
            .IsNotEqualTo(different));
    }

    [TestMethod]
    public void AssertThatObject_IsNotEqualTo_ShouldFail_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new MyClass();
        var same = subject;

        // Act & Assert
        Validate.Fail(() => Assert
            .That(subject)
            .IsNotEqualTo(same));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldPass_WhenObjectsAreSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = subject;

        // Act & Assert
        Validate.Pass(() => Assert
            .That(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldFail_WhenObjectsAreNotSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = new MyClass();

        // Act & Assert
        Validate.Fail(() => Assert
            .That(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldPass_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Validate.Pass(() => Assert
            .That(subject!)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldFail_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Validate.Fail(() => Assert
            .That(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldPass_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Validate.Pass(() => Assert
            .That(subject)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldFail_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Validate.Fail(() => Assert
            .That(subject!)
            .IsNotNull());
    }
}

internal class MyClass
{
    public MyClass()
    {
    }
}
