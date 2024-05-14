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

    [TestMethod]
    public void TestConstructor_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        double[] data = [];
        var asyncVectorWithEmptyVector = () => new AsyncVector<double>(data);

        // Act & Assert
        Validate.Success(() => Zentient.Tests.Assert
            .That(asyncVectorWithEmptyVector.Vector)
            .);
    }

    // Tests if the constructor returns an empty vector when provided with an empty data dictionary.
    [TestMethod]
    public void TestConstructor_EmptyData_ReturnsEmptyVector()
    {

    }

    /*
    3. **TestDotProductAsync_SameVector_ReturnsSquareMagnitude**: Tests if the dot product of a vector with itself returns the square of its magnitude.
    4. **TestDotProductAsync_OrthogonalVectors_ReturnsZero**: Tests if the dot product of orthogonal vectors returns zero.
    5. **TestDotProductAsync_EmptyVector_ReturnsZero**: Tests if the dot product of a vector with an empty vector returns zero.
    6. **TestAddAsync_EmptyVectors_ReturnsEmptyVector**: Tests if adding two empty vectors returns an empty vector.
    7. **TestAddAsync_VectorWithEmptyVector_ReturnsSameVector**: Tests if adding a vector with an empty vector returns the same vector.
    8. **TestAddAsync_VectorWithItself_ReturnsDoubledVector**: Tests if adding a vector with itself returns a vector with each element doubled.
    9. **TestAddAsync_VectorWithZeroVector_ReturnsSameVector**: Tests if adding a vector with a zero vector returns the same vector.
    10. **TestAddAsync_VectorWithOppositeVectors_ReturnsZeroVector**: Tests if adding a vector with its opposite vector returns a zero vector.
    11. **TestAddAsync_VectorWithOrthogonalVectors_ReturnsVectorWithSum**: Tests if adding two orthogonal vectors returns a vector with the sum of their elements.
    12. **TestCancellation_DotProductAsync_CancellationRequested_ThrowsOperationCanceledException**: Tests if requesting cancellation during dot product computation throws OperationCanceledException.
    13. **TestCancellation_AddAsync_CancellationRequested_ThrowsOperationCanceledException**: Tests if requesting cancellation during addition of vectors throws OperationCanceledException.
    14. **TestPerformance_LargeVectors_CompletesWithinTimeLimit**: Tests if dot product computation completes within a specified time limit for large vectors.
    15. **TestConcurrency_MultipleThreads_ComputeDotProductCorrectly**: Tests if dot product computation is correct when performed concurrently by multiple threads.
    16. **TestCompatibility_OtherTypes_ComputeDotProductCorrectly**: Tests if dot product computation is correct when using other numeric types (e.g., double, float) with AsyncVector.
     */
}
