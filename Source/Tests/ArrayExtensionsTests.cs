using Zentient.Tests;
using Zentient.Extensions;

namespace Tests;

[TestClass]
public class ArrayExtensionsTests
{
    public Assert Assert { get => Assert.Instance; }

    private readonly int[] _empty = new int[0];
    private readonly int[] _single = { 1 };
    private readonly int[] _small = { 1, 2, 3, 4 };
    private readonly int[] _medium = { 1, 2, 3, 4, 5, 6, 7, 8 };
    private readonly int[] _large = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    private readonly int[] _ordered = new[] { 1, 2, 3, 4, 5 };
    private readonly int[] _unordered = new[] { 3, 1, 4, 2, 5 };
    private readonly int[] _oddnumbers = new[] { 1, 3, 5, 7, 9 };
    private readonly int[] _evennumbers = new[] { 2, 4, 6, 8, 10 };

    [TestMethod]
    public void TestShuffle()
    {
        // Arrange
        var actual = _ordered.ToArray();
        var expected = _ordered.ToArray();

        // Act
        actual = actual.Shuffle().ToArray();

        // Act & Assert
        Assert.Fail(() =>
            Assert.That(actual).SequenceEquals(expected));
    }

    [TestMethod]
    public void TestShuffle_ShouldPass_WhenUsingSingleItem()
    {
        // Arrange
        var actual = _single.ToArray();
        var expected = _single.ToArray();

        // Act
        actual = actual.Shuffle().ToArray();

        // Act & Assert
        Assert.Pass(() =>
            Assert.That(actual).SequenceEquals(expected));
    }

    [TestMethod]
    public void Chunk_ShouldReturnEmptyArray_WhenInputArrayIsEmpty_ShouldPass()
    {
        var actual = _empty.Chunk(2);

        Assert.Pass(() => actual.Length == 0);
    }

    [TestMethod]
    public void Chunk_ShouldReturnSingleArray_WhenInputArrayContainsOneElement_ShouldPass()
    {
        var actual = _single.Chunk(2);

        Assert.Pass(() => Assert
            .That(actual).CountEquals(1));

        Assert.Pass(() =>
            Assert.That<int[]>(actual).AllItemsSatisfy(x => x.Length == 1));
    }

    [TestMethod]
    public void Chunk_ShouldSplitArrayIntoChunks_WhenChunkSizeIsPerfectDivisor_ShouldPass()
    {
        var actual = _small.Chunk(2);

        Assert.Pass(() => Assert
            .That(actual).CountEquals(2));

        Assert.Pass(() =>
            Assert.That<int[]>(actual).AllItemsSatisfy(x => x.Length == 2));
    }

    [TestMethod]
    public void Chunk_ShouldSplitArrayIntoChunks_WhenChunkSizeIsNotPerfectDivisor_ShouldPass()
    {
        var actual = _small.Chunk(3);

        Assert.Pass(() => Assert
            .That(actual).CountEquals(2));

        Assert.Pass(() =>
            Assert.That<int[]>(actual).AllItemsSatisfy(x => x.Length == 1 || x.Length == 3));
    }

    [TestMethod]
    public void Chunk_ShouldSplitLargeArrayIntoChunks_WhenChunkSizeIsLarge_ShouldPass()
    {
        var actual = _large.Chunk(5);

        Assert.Pass(() => Assert
            .That(actual).CountEquals(3));

        Assert.Pass(() =>
            Assert.That<int[]>(actual).AllItemsSatisfy(x => x.Length == 3 || x.Length == 5));
    }

    [TestMethod]
    public void Chunk_ShouldSplitArrayIntoChunks_WhenChunkSizeIsOne_ShouldPass()
    {
        var actual = _medium.Chunk(1);

        Assert.Pass(() => Assert
            .That(actual).CountEquals(8));

        Assert.Pass(() =>
            Assert.That<int[]>(actual).AllItemsSatisfy(x => x.Length == 1));
    }

    [TestMethod]
    public void Chunk_ShouldPass_WhenChunkSize_IsValid()
    {
        var actual = _small;
        Assert.Pass(() =>
            Assert.That(() => actual.Chunk(2)).DoesNotThrow<ArgumentOutOfRangeException>());
    }

    [TestMethod]
    public void Chunk_ShouldFail_WhenChunkSize_IsInvalid()
    {
        var actual = _small;
        Assert.Fail(() =>
            Assert.That(() => actual.Chunk(-2)).DoesNotThrow<ArgumentOutOfRangeException>());
    }


    // IndexWhere
    [TestMethod]
    public void IndexWhere_ShouldReturnNegativeOne_WhenInputArrayIsEmpty_ShouldPass() 
    {
        Assert.Pass(_empty.IndexWhere(p => p == 0) == -1);
    }
    [TestMethod]
    public void IndexWhere_ShouldReturnNegativeOne_WhenNoElementsInArraySatisfy_ShouldPass() 
    {
        Assert.Pass(_single.IndexWhere(p => p == 0) == -1);
    }
    [TestMethod]
    public void IndexWhere_ShouldReturnZero_WhenAllElementsInArraySatisfy_ShouldPass() 
    {
        Assert.Pass(_small.IndexWhere(p => p > 0) == 0);
    }
    [TestMethod]
    public void IndexWhere_ShouldReturnCorrectIndex_WhenSomeElementsInArraySatisfy_ShouldPass() 
    {
        Assert.Pass(_small.IndexWhere(p => p == 2) == 1);
    }
    [TestMethod]
    public void IndexWhere_ShouldThrowArgumentNullException_WhenInputArrayIsNull() 
    {
        int[]? actual = null!;
        Action action = () => actual.IndexWhere(p => p == 0);
        Assert.That(action).Throws<ArgumentNullException>();
    }
    [TestMethod]
    public void IndexWhere_ShouldThrowArgumentNullException_WhenPredicateIsNull() 
    {
        var actual = _small;
        Func<int, bool> predicate = null!;
        Action action = () => actual.IndexWhere(predicate);
        Assert.That(action).Throws<ArgumentNullException>();
    }

    // LastIndexOf
    [TestMethod]
    public void LastIndexWhere_ShouldReturnNegativeOne_WhenInputArrayIsEmpty_ShouldPass() 
    {
        Assert.Pass(_empty.LastIndexWhere(p => p == 1) == -1);
    }

    [TestMethod]
    public void LastIndexWhere_ShouldReturnZero_WhenNoElementsInArraySatisfy_ShouldPass() 
    {
        Assert.Pass(_small.LastIndexWhere(p => p == 0) == -1);
    }
    
    [TestMethod]
    public void LastIndexWhere_ShouldReturnCorrectIndex_WhenAllElementsInArraySatisfy_ShouldPass() 
    {
        var actual = _small;
        Assert.Pass(actual.LastIndexWhere(p => p > 0) == actual.Length - 1);
    }
    
    [TestMethod]
    public void LastIndexWhere_ShouldReturnCorrectIndex_WhenSomeElementsInArraySatisfy_ShouldPass() 
    {
        Assert.Pass(_medium.LastIndexWhere(p => p.IsOdd()) == 6);
    }

    [TestMethod]
    public void LastIndexWhere_ShouldThrowArgumentNullException_WhenInputArrayIsNull() 
    {
        int[]? actual = null!;
        Action action = () => actual.IndexWhere(p => p == 0);
        Assert.That(action).Throws<ArgumentNullException>();
    }

    [TestMethod]
    public void LastIndexWhere_ShouldThrowArgumentNullException_WhenPredicateIsNull() 
    {
        int[] actual = _small;
        Func<int, bool> predicate = null!;
        Action action = () => actual.IndexWhere(predicate);
        Assert.That(action).Throws<ArgumentNullException>();
    }

    // Distinct
    [TestMethod]
    public void Distinct_ShouldReturnEmptyArray_WhenInputArrayIsEmpty_ShouldPass()
    {
        Assert.Pass(_empty.Distinct().Length == 0);

    }

    [TestMethod]
    public void Distinct_ShouldReturnSameArray_WhenNoDuplicatesExist_ShouldPass()
    {
        Assert.Pass(_small.Distinct().Length == _small.Length);
    }

    [TestMethod]
    public void Distinct_ShouldReturnArrayWithOnlyUniqueElements_WhenDuplicatesExist_ShouldPass()
    {
        int[] actual = [1, 2, 3, 2, 4];
        var expected = _small;
        Assert.Pass(actual.Distinct().ToList().SequenceEqual(expected));
    }
 
    [TestMethod]
    public void Distinct_ShouldThrowArgumentNullException_WhenInputArrayIsNull()
    {
        int[]? actual = null!;
        Action action = () => actual.Distinct();
        Assert.That(action).Throws<ArgumentNullException>();
    }

    // Sum
    [TestMethod]
    public void Sum_ShouldReturnDefaultValue_WhenInputArrayIsEmpty_ShouldPass() 
    {
        var actual = _empty.Sum();
        var expected = 0;
        Assert.Pass(actual == expected);
    }
    [TestMethod]
    public void Sum_ShouldReturnCorrectSum_WhenAllElementsInArrayArePositive_ShouldPass() 
    {
        var actual = _small.Sum();
        var expected = 1 + 2 + 3 + 4;
        Assert.Pass(actual == expected);
    }

    [TestMethod]
    public void Sum_ShouldReturnCorrectSum_WhenAllElementsInArrayAreNegative_ShouldPass() 
    {
        var actual = _small.Select(p => -p).Sum();
        var expected = -1 + -2 + -3 + -4;
        Assert.Pass(actual == expected);
    }

    [TestMethod]
    public void Sum_ShouldReturnCorrectSum_WhenInputArrayContainsPositiveAndNegativeNumbers_ShouldPass() 
    {
        int[] array = { 1, -2, 3, -4 };
        var actual = array.Sum();
        int expected = 1 - 2 + 3 - 4;
        Assert.Pass(actual == expected);
    }
    
    [TestMethod]
    public void Sum_ShouldReturnCorrectSum_WhenDuplicatesExist_ShouldPass() 
    {
        int[] array = { 1, -2, 3, -2, -4, 3 };
        var actual = array.Sum();
        int expected = 1 - 2 + 3 - 2 - 4 + 3;
        Assert.Pass(actual == expected);
    }

    [TestMethod]
    public void Sum_ShouldReturnCorrectSum_WithDifferentNumericTypes_ShouldPass() 
    {
        // TODO:
    }

    [TestMethod]
    public void Sum_ShouldThrowArgumentNullException_WhenInputArrayIsNull() 
    {
        int[]? actual = null!;
        Action action = () => actual.Sum();
        Assert.That(action).Throws<ArgumentNullException>();
    }
}
