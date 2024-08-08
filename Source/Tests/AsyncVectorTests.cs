//
// Class: ExceptionAssertionBuilderTests
// 
// Description:
// Provides a set of test methods for the class <see chref="AsyncAssertionBuilder"/>. 
// 
// MIT License
// 
// Copyright (c) 2024 Ulf Bourelius
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System.Collections.Immutable;
using Zentient.Tests;
using Zentient.Vectors;

namespace Tests;

public class AsyncVectorTests
{
    public Assert Assert {  get => Assert.Instance; }

    // Tests if the constructor throws ArgumentNullException when provided with null data.
    [TestMethod]
    public void TestConstructor_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        var asyncVectorWithNullArgument = () => new AsyncVector<double>(null!);

        var builder = Assert.That(() => new AsyncVector<double>(null!));

        // Act & Assert
        Validate.Pass(() => Assert
            .That(() => new AsyncVector<double>(null!))
            .Throws<ArgumentNullException>("Should throw ArgumentNullException"));
    }

    // Tests if the constructor returns an empty vector when provided with an empty data dictionary.
    [TestMethod]
    public void TestConstructor_EmptyData_ReturnsEmptyVector()
    {
        // Arrange
        double[] data = [];
        ICollection<double> emptyCollection = data.ToArray();
        var asyncVectorWithEmptyVector = new AsyncVector<double>(data);
        var assert = Assert.That<double>(asyncVectorWithEmptyVector.Vector);

        // Act & Assert
        Validate.Pass(() => 
            Assert.That<double>(asyncVectorWithEmptyVector.Vector).SequenceEquals(emptyCollection));
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
        Validate.Pass(() => Assert
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
        Validate.Pass(() => Assert
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
        Validate.Pass(() => Assert
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
        Validate.Pass(() => Assert
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
        Validate.Pass(() => Assert
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
        Validate.Pass(() => Assert
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
