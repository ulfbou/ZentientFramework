using Zentient.Tests;

namespace Tests;
#if false
[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
public class ThrowsAssertsTests
{
    // Test for Throws<T>(Func<object> value)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public void Throws_ThrowsExpectedException()
    {
        // Arrange
        Func<object> methodToTest = () => throw new ArgumentException("Expected exception");

        // Act & Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsException<ArgumentException>(() => Zentient.Tests.Assert.Throws<ArgumentException>(() => methodToTest()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public void Throws_DoesNotThrowUnexpectedException()
    {
        // Arrange
        Func<object> methodToTest = () => throw new InvalidOperationException("Unexpected exception");

        // Act & Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsException<ArgumentException>(() => Zentient.Tests.Assert.Throws<ArgumentException>(() => methodToTest()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    [ExpectedException(typeof(FailedTestException))]
    public void Throws_NoExceptionThrown()
    {
        // Arrange
        Func<object> methodToTest = () => null; // No exception expected

        // Act
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsException<ArgumentException>(() => methodToTest());
    }

    // Test for ThrowsAsync<T>(Func<Task<object>> value)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public async Task ThrowsAsync_ThrowsExpectedException()
    {
        // Arrange
        Func<Task<object>> methodToTestAsync = async () => { throw new ArgumentException("Expected exception"); };

        // Act & Assert
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() => Zentient.Tests.Assert.ThrowsAsync<ArgumentException>(() => methodToTestAsync()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public async Task ThrowsAsync_DoesNotThrowUnexpectedException()
    {
        // Arrange
        Func<Task<object>> methodToTestAsync = async () => { throw new InvalidOperationException("Unexpected exception"); };

        // Act & Assert
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Zentient.Tests.Assert.ThrowsAsync<ArgumentException>(() => methodToTestAsync()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    [ExpectedException(typeof(FailedTestException))]
    public async Task ThrowsAsync_NoExceptionThrown()
    {
        // Arrange
        Func<Task<object>> methodToTestAsync = async () => await Task.FromResult<object>(null); // No exception expected

        // Act
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() => methodToTestAsync());
    }

    // Test for Throws<T>(Func<Task> value)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public async Task ThrowsAsync_Task_ThrowsExpectedException()
    {
        // Arrange
        Func<Task> methodToTestAsync = async () => { throw new ArgumentException("Expected exception"); };

        // Act & Assert
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() => Zentient.Tests.Assert.ThrowsAsync<ArgumentException>(() => methodToTestAsync()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    public async Task ThrowsAsync_Task_DoesNotThrowUnexpectedException()
    {
        // Arrange
        Func<Task> methodToTestAsync = async () => { throw new InvalidOperationException("Unexpected exception"); };

        // Act & Assert
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() => Zentient.Tests.Assert.ThrowsAsync<ArgumentException>(() => methodToTestAsync()));
    }

    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
    [ExpectedException(typeof(FailedTestException))]
    public async Task ThrowsAsync_Task_NoExceptionThrown()
    {
        // Arrange
        Func<Task> methodToTestAsync = async () => { await Task.CompletedTask; }; // No exception expected

        // Act
        await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() => methodToTestAsync());
    }
}
#endif
