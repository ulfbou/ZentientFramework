using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using Zentient.Tests;
using Zentient.Vectors;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    // Tests if the constructor returns an empty vector when provided with an empty data dictionary.
    [TestMethod]
    public void TestConstructor_EmptyData_ReturnsEmptyVector()
    {
        // Arrange
        double[] data = [];
        var asyncVectorWithEmptyVector = new AsyncVector<double>(data);

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<object>(asyncVectorWithEmptyVector.Vector.Length)
            .IsEqualTo(0));
    }

    // Tests if the dot product of a vector with itself returns the square of its magnitude.
    [TestMethod]
    public async Task TestDotProductAsync_SameVector_ReturnsSquareMagnitude()
    {
        // Arrange
        double[] data = [0, 2];
        AsyncVector<double> vector = new AsyncVector<double>(data);

        // Act 
        double result = await vector.DotProductAsync(vector);
        double expectedResult = 0 * 0 + 2 * 2;

        // Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<double>(result)
            .IsEqualTo(expectedResult));
    }

    // Tests if the dot product of orthogonal vectors returns zero.
    [TestMethod]
    public async Task TestDotProductAsync_OrthogonalVectors_ReturnsZero()
    {
        // Arrange
        double[] data1 = [0, 2];
        double[] data2 = [2, 0];
        AsyncVector<double> vector1 = new AsyncVector<double>(data1);
        AsyncVector<double> vector2 = new AsyncVector<double>(data2);

        // Act 
        double result = await vector1.DotProductAsync(vector2);
        double expectedResult = 0 * 2 + 2 * 0;

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<double>(result)
            .IsEqualTo(expectedResult));
    }

    // Tests if the dot product of a vector with an empty vector returns zero.
    [TestMethod]
    public async Task TestDotProductAsync_EmptyVector_ReturnsZero()
    {
        // Arrange
        double[] data1 = [];
        double[] data2 = [2, 0];
        AsyncVector<double> vector1 = new AsyncVector<double>(data1);
        AsyncVector<double> vector2 = new AsyncVector<double>(data2);

        // Act 
        double result = await vector1.DotProductAsync(vector2);
        double expectedResult = 0 * 2 + 2 * 0;

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<double>(result)
            .IsEqualTo(expectedResult));
    }

    // Tests if adding two empty vectors returns an empty vector.
    [TestMethod]
    public async Task TestAddAsync_EmptyVectors_ReturnsEmptyVector()
    {
        // Arrange
        double[] data1 = [];
        double[] data2 = [];
        AsyncVector<double> vector1 = new AsyncVector<double>(data1);
        AsyncVector<double> vector2 = new AsyncVector<double>(data2);

        // Act 
        AsyncVector<double> result = await vector1.AddAsync(vector2);
        ImmutableArray<double> expectedResult = [];

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That(result.Vector.SequenceEqual(expectedResult))
            .IsTrue("The vectors should be equal"));;
    }

    // Tests if adding a vector with an empty vector returns the same vector.
    [TestMethod]
    public async Task TestAddAsync_VectorWithEmptyVector_ReturnsSameVector()
    {
        // Arrange
        double[] data1 = [1, 2];
        double[] data2 = [];
        AsyncVector<double> vector1 = new AsyncVector<double>(data1);
        AsyncVector<double> vector2 = new AsyncVector<double>(data2);

        // Act 
        AsyncVector<double> result = await vector1.AddAsync(vector2);
        ImmutableArray<double> expectedResult = data1.ToImmutableArray();

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That(result.Vector.SequenceEqual(expectedResult))
            .IsTrue("The vectors should be equal"));
    }

    // Tests if adding a vector with itself returns a vector with each element doubled.
    [TestMethod]
    public async Task TestAddAsync_VectorWithItself_ReturnsDoubledVector()
    {
        // Arrange
        double[] data = [1, 2];
        double[] expectedData = [data[0] * 2, data[1] * 2];
        AsyncVector<double> vector = new AsyncVector<double>(data);
        AsyncVector<double> expectedResult = new AsyncVector<double>(expectedData);

        // Act 
        AsyncVector<double> result = await vector.AddAsync(vector);

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That<AsyncVector<double>>(result)
            .IsEqualTo(expectedResult));
    }

#if false
    // Tests if adding a vector with a zero vector returns the same vector.
    [TestMethod]
    public async Task TestAddAsync_VectorWithZeroVector_ReturnsSameVector()
    {
    }

    // Tests if adding a vector with its opposite vector returns a zero vector.
    [TestMethod]
    public async Task TestAddAsync_VectorWithOppositeVectors_ReturnsZeroVector()
    {
    }

    // Tests if adding two orthogonal vectors returns a vector with the sum of their elements.
    [TestMethod]
    public async Task TestAddAsync_VectorWithOrthogonalVectors_ReturnsVectorWithSum()
    {
    }

    // Tests if requesting cancellation during dot product computation throws OperationCanceledException.
    [TestMethod]
    public async Task TestCancellation_DotProductAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
    }

    // Tests if requesting cancellation during addition of vectors throws OperationCanceledException.
    [TestMethod]
    public async Task TestCancellation_AddAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
    }

    // Tests if dot product computation completes within a specified time limit for large vectors.
    [TestMethod]
    public async Task TestPerformance_LargeVectors_CompletesWithinTimeLimit()
    {
    }

    // Tests if dot product computation is correct when performed concurrently by multiple threads.
    [TestMethod]
    public async Task TestConcurrency_MultipleThreads_ComputeDotProductCorrectly()
    {
    }

    // Tests if dot product computation is correct when using other numeric types (e.g., double, float) with AsyncVector.
    [TestMethod]
    public async Task TestCompatibility_OtherTypes_ComputeDotProductCorrectly()
    {
    }
#endif
}
