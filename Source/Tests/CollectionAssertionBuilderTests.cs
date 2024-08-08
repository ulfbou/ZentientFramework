//
// Class: CollectionAssertionBuilderTests
// 
// Description:
// Provides a set of test methods for the class <see chref="CollectionAssertionBuilder"/>. 
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

using Zentient.Tests;

namespace Tests;
/// <summary>
/// Specifies that a method is a test method expects that an Exception is thrown..
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ExpectedExceptionAttribute(Type type) : Attribute
{
    private readonly Type _type = type;
}

public class CollectionAssertionBuilderTests
{
    public Assert Assert { get => Assert.Instance; }

    private ICollection<int> _emptyCollection = null!;
    private ICollection<int> _nonEmptyCollection = null!;
    private ICollection<int> _orderedArray = null!;
    private ICollection<int> _orderedArray2 = null!;
    private ICollection<int> _unorderedArray = null!;
    private ICollection<int> _unorderedArray2 = null!;

    [TestSetup]
    public void Setup()
    {
        _emptyCollection = new List<int>();
        _nonEmptyCollection = new List<int> { 1, 2, 3 };
        _orderedArray = [1, 2, 3, 4, 5];
        _orderedArray2 = [1, 2, 3, 4, 5, 6];
        _unorderedArray = [4, 1, 5, 3, 2];
        _unorderedArray2 = [4, 1, 3, 6, 2, 5];
    }

    [TestMethod]
    public void TestCountEquals_NonEmptyCollection_ShouldPass()
    {
        Assert.Pass(() => Assert
            .That(_nonEmptyCollection).CountEquals(3));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestCountEquals_NonEmptyCollection_ShouldFail()
    {
        Assert.Fail(() => Assert
        .That(_nonEmptyCollection).CountEquals(2));
    }

    [TestMethod]
    public void TestIsEmpty_EmptyCollection_ShouldPass()
    {
        Assert.Pass(() => Assert.That(_emptyCollection).IsEmpty());
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestIsEmpty_NonEmptyCollection_ShouldFail()
    {
        Assert.Fail(() =>
        Assert.That(_nonEmptyCollection).IsEmpty());
    }

    [TestMethod]
    public void TestIsNotEmpty_NonEmptyCollection_ShouldPass()
    {
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).IsNotEmpty());
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestIsNotEmpty_EmptyCollection_ShouldFail()
    {
        Assert.Fail(() =>
            Assert.That(_emptyCollection).IsNotEmpty());
    }

    [TestMethod]
    public void TestContains_NonEmptyCollection_ShouldPass()
    {
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).Contains(2));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestContains_NonEmptyCollection_ShouldFail()
    {
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).Contains(4));
    }

    [TestMethod]
    public void TestDoesNotContain_NonEmptyCollection_ShouldPass()
    {
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).DoesNotContain(4));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestDoesNotContain_NonEmptyCollection_ShouldFail()
    {
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).DoesNotContain(2));
    }

    [TestMethod]
    public void TestSequenceEquals_NonEmptyCollection_ShouldPass()
    {
        var collectionToCompare = new List<int> { 1, 2, 3 };
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).SequenceEquals(collectionToCompare));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestSequenceEquals_NonEmptyCollection_ShouldFail()
    {
        var collectionToCompare = new List<int> { 3, 2, 1 };
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).SequenceEquals(collectionToCompare));
    }

    [TestMethod]
    public void AssertThatArray_CountEquals_ShouldPass_WhenUsingArrayLength()
    {
        int[] array = [2, 3, 5, 7, 11];
        Assert.Pass(() =>
            Assert.That(array).CountEquals(array.Length)
        );
    }


    [TestMethod]
    public void TestIsSubsetOf_NonEmptyCollection_ShouldPass()
    {
        var superset = new List<int> { 1, 2, 3, 4 };
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).IsSubsetOf(superset));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestIsSubsetOf_NonEmptyCollection_ShouldFail()
    {
        var superset = new List<int> { 1, 2 };
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).IsSubsetOf(superset));
    }

    [TestMethod]
    public void TestIsSupersetOf_NonEmptyCollection_ShouldPass()
    {
        var subset = new List<int> { 1, 2 };
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).IsSupersetOf(subset));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestIsSupersetOf_NonEmptyCollection_ShouldFail()
    {
        var subset = new List<int> { 4, 5 };
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).IsSupersetOf(subset));
    }

    [TestMethod]
    public void TestIntersectsWith_NonEmptyCollection_ShouldPass()
    {
        var otherCollection = new List<int> { 2, 4 };
        Assert.Pass(() =>
            Assert.That(_nonEmptyCollection).IntersectsWith(otherCollection));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestIntersectsWith_NonEmptyCollection_ShouldFail()
    {
        var otherCollection = new List<int> { 4, 5 };
        Assert.Fail(() =>
            Assert.That(_nonEmptyCollection).IntersectsWith(otherCollection));
    }

    [TestMethod]
    public void TestHasUniqueItems_NonEmptyCollection_ShouldPass()
    {
        var uniqueCollection = new List<int> { 1, 2, 3 };
        Assert.Pass(() =>
            Assert.That((IEnumerable<int>)uniqueCollection).HasUniqueItems());
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestHasUniqueItems_NonEmptyCollection_ShouldFail()
    {
        var duplicateCollection = new List<int> { 1, 2, 2, 3 };

        Assert.Fail(() =>
            Assert.That<int>(duplicateCollection).HasUniqueItems());
    }

    [TestMethod]
    public void TestHasDuplicates_NonEmptyCollection_ShouldPass()
    {
        var duplicateCollection = new List<int> { 1, 2, 2, 3 };
        Assert.Pass(() =>
            Assert.That((IEnumerable<int>)duplicateCollection).HasDuplicates(),
            "Should have duplicates");
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestHasDuplicates_NonEmptyCollection_ShouldFail()
    {
        var uniqueCollection = new List<int> { 1, 2, 3 };
        Assert.Fail(() =>
            Assert.That((IEnumerable<int>)uniqueCollection).HasDuplicates(),
            "Should not have duplicates");
    }

    [TestMethod]
    public void TestAreEqualIgnoringOrder_ValidCollections_ShouldPass()
    {
        var actual = _orderedArray.ToArray();
        var expected = _unorderedArray.ToList();
        Assert.Pass(() =>
                  Assert
                  .That<int>(actual)
                  .AreEqualIgnoringOrder(expected));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestAreEqualIgnoringOrder_InvalidCollections_ShouldFail()
    {
        var actual = _orderedArray.ToArray();
        var expected = _unorderedArray2.ToList();
        Assert.Fail(() =>
                  Assert
                  .That<int>(actual)
                  .AreEqualIgnoringOrder(expected));
    }

    [TestMethod]
    public void TestAreEquivalent_ValidCollections_ShouldPass()
    {
        var actual = _orderedArray;
        var expected = actual.ToList();
        Assert.Pass(() =>
                  Assert.That<int>(actual).AreEquivalent(expected));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestAreEquivalent_InvalidCollections_ShouldFail()
    {
        var actual = _orderedArray.ToArray();
        var expected = _unorderedArray2.ToList();
        Assert.Fail(() =>
                  Assert.That<int>(actual).AreEquivalent(expected));
    }

    [TestMethod]
    public void TestAreDisjoint_ValidCollections_ShouldPass()
    {
        var actual = _orderedArray;
        var expected = actual.ToList();
        Assert.Pass(() =>
            Assert.That<int>(actual).AreEquivalent(expected));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestAreDisjoint_InvalidCollections_ShouldFail()
    {
        var actual = _orderedArray.ToArray();
        var expected = _unorderedArray2.ToList();
        Assert.Fail(() =>
            Assert.That<int>(actual).AreEquivalent(expected));
    }

    [TestMethod]
    public void TestHasMinLength_ValidLength_ShouldPass()
    {
        var actual = _orderedArray;
        Assert.Pass(() =>
          Assert.That<int>(actual).HasMinLength(actual.Count()));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestHasMinLength_InvalidLength_ShouldFail()
    {
        var actual = _orderedArray;
        Assert.Fail(() =>
            Assert.That<int>(actual).HasMinLength(actual.Count() + 1));
    }

    [TestMethod]
    public void TestHasMaxLength_ValidLength_ShouldPass()
    {
        var actual = _orderedArray;
        Assert.Pass(() =>
            Assert.That<int>(actual).HasMaxLength(actual.Count()));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestHasMaxLength_InvalidLength_ShouldFail()
    {
        var actual = _orderedArray;
        Assert.Fail(() =>
            Assert.That<int>(actual).HasMaxLength(actual.Count() - 1));
    }

    [TestMethod]
    public void TestHasItemSatisfying_ValidPredicate_ShouldPass()
    {
        var actual = _orderedArray;
        Assert.Pass(() =>
            Assert.That<int>(actual).HasItemSatisfying(x => x > 3));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestHasItemSatisfying_InvalidPredicate_ShouldFail()
    {
        var actual = _orderedArray;
        Assert.Fail(() =>
            Assert.That<int>(actual).HasItemSatisfying(x => x > 6));
    }

    [TestMethod]
    public void TestAllItemsSatisfying_ValidPredicate_ShouldPass()
    {
        var actual = _orderedArray;
        Assert.Pass(() =>
            Assert.That<int>(actual).AllItemsSatisfy(x => x > 0));
    }

    [TestMethod]
    [ExpectedException(typeof(AssertionFailureException))]
    public void TestAllItemsSatisfying_InvalidPredicate_ShouldFail()
    {
        var actual = _orderedArray;
        Assert.Fail(() =>
            Assert.That<int>(actual).AllItemsSatisfy(x => x > 3));
    }

    [TestMethod]
    public void TestAreEqualByProperty_1_ValidPredicate_ShouldPass()
    {
        var collection1 = new int[] { 1, 2, 3, 4, 5 };
        var collection2 = new List<int> { 2, 4, 6, 8, 10 };

        // Create a collection of tuples representing (item from collection1, item from collection2)
        var actual = collection1.Zip(collection2, (x, y) => (x, y));
        var expected = collection1.Zip(collection2, (x, y) => (x, x));

        Assert.Pass(() =>
            Assert.That(actual).AreEqualByProperty(expected.ToList(), tuple => tuple.Item1));

    }


    [TestMethod]
    public void TestAreEqualByProperty_1_InvalidPredicate_ShouldFail()
    {
        var collection1 = new int[] { 1, 2, 3, 4, 5 };
        var collection2 = new List<int> { 2, 4, 6, 8, 10 };

        // Create a collection of tuples representing (item from collection1, item from collection2)
        var actual = collection1.Zip(collection2, (x, y) => (x, y));
        var expected = collection1.Zip(collection2, (x, y) => (x, x));

        Assert.Fail(() =>
            Assert.That(actual).AreEqualByProperty(expected.ToList(), tuple => tuple.Item2));
    }

    struct Pair(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }
}
