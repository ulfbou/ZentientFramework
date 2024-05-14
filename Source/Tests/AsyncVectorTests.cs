using Zentient.Tests;
using Zentient.Vectors;

namespace Tests;

[TestClass]
public class AsyncVectorTests
{
    // Tests if the constructor throws ArgumentNullException when provided with null data.
    [TestMethod]
    public void TestConstructor_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        var asyncVectorWithNullArgument = () => new AsyncVector<double>(null!);

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That(() => new AsyncVector<double>(null!))
            .Throws<ArgumentNullException>());
    }
}
