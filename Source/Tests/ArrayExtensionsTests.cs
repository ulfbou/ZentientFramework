using Zentient.Tests;
using Zentient.Extensions;

namespace Tests;

[TestClass]
public class ArrayExtensionsTests
{
    public Assert Assert { get => Assert.Instance; }

    private int[] _ordered = null!;
    private int[] _oddnumbers = null!;
    private int[] _evennumbers = null!;
    private int[] _unordered = null!;

    [TestSetup]
    public void Setup()
    {
        _ordered = new[] { 1, 2, 3, 4, 5 };
        _unordered = new[] { 3, 1, 4, 2, 5 };
        _oddnumbers = new[] { 1, 3, 5, 7, 9 };
        _evennumbers = new[] { 2, 4, 6, 8, 10 };
    }

    // Tests if the constructor throws ArgumentNullException when provided with null data.
    [TestMethod]
    public void TestShuffle()
    {
        // Arrange
        var actual = _ordered.ToArray();
        var expected = _ordered.ToArray();

        // Act
        actual.Shuffle();

        // Act & Assert
        Assert.Pass(() =>
            Assert.That(actual).SequenceEquals(expected));
    }
}
