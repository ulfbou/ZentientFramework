//
// File: Assert.cs
//
// Description: Assert Methods.
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

namespace Zentient.Tests;

public static class Assert
{
    public static void Equals<T>(T result, T expectedResult) where T : IEquatable<T>
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(expectedResult);

        if (!result.Equals(expectedResult))
        {
            throw new FailedTestException($"Failed Test. Expected {expectedResult}, but got {result}.");
        }
    }

    /// <summary>
    /// Asserts that a delegate does not throw an exception.
    /// </summary>
    /// 
    public static void DoesNotThrow(Func<object> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));
        try
        {
            value();
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Throws {ex.GetType().Name} when expecting no exception to be thrown.", ex);
        }
    }

    /// <summary>
    /// Asserts that an async delegate does not throw any exceptions.
    /// </summary>
    /// 
    public static async Task DoesNotThrowAsync(Func<Task<object>> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));
        try
        {
            await value();
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Throws {ex.GetType().Name} when expecting no exception to be thrown.", ex);
        }
    }

    /// <summary>
    /// Asserts that a delegate throws exception <see cref="T">exception</see>.
    /// </summary>
    /// 
    public static void Throws<T>(Func<object> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));
        throw new NotImplementedException();
    }

    public static void ThrowsAny(Func<object> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));
        throw new NotImplementedException();
    }

    public static async Task ThrowsAnyAsync(Func<Task<object>> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));
        throw new NotImplementedException();
    }

    public static async Task ThrowsAsync<T>(Func<Task> value) where T : Exception
    {
        try
        {
            await value();
        }
        catch (T)
        {
            // Expected exception of type T was thrown, no action needed
            return;
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Expected {nameof(T)} to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        // If control reaches here, it means the expected exception T was not thrown
        throw new FailedTestException($"Failed test: Expected exception to be thrown, but it wasn't.");
    }

    public static void AreEqual(object? expectedObject, object? actualObject)
    {
        AreEqual<object>(expectedObject, actualObject);
    }

    public static void AreEqual<T>(T expectedObject, T actualObject)
    {
        ArgumentNullException.ThrowIfNull(expectedObject);
        ArgumentNullException.ThrowIfNull(actualObject);

        if (!expectedObject.Equals(actualObject))
        {
            throw new FailedTestException($"Failed test: Expected {expectedObject}, but got {actualObject}.");
        }
    }

    public static void AreNotEqual(object? notExpectedObject, object actualObject)
    {
        AreNotEqual<object>(notExpectedObject, actualObject);
    }

    public static void AreNotEqual<T>(T notExpectedObject, T actualObject)
    {
        try
        {
            AreEqual<T>(notExpectedObject, actualObject);
        }
        catch (FailedTestException ex)
        {
            return;
        }
        catch { }
        throw new FailedTestException($"Failed test: Expected to not receive {notExpectedObject}, but got {actualObject}.");
    }

    public static void IsTrue(bool check)
    {
        if (check) throw new FailedTestException($"Failed test: Expected true condition, but got false.");
    }

    public static void IsFalse(bool check)
    {
        if (!check) throw new FailedTestException($"Failed test: Expected false condition, but got true.");
    }

    public static void ReferenceEquals<T>(T expectedObject, T actualObject)
    {
        if (!object.ReferenceEquals(expectedObject, actualObject))
        {
            throw new FailedTestException($"Expected two objects to share references.");
        }
    }

    /// <summary>
    /// Asserts that two objects refer to the same instance.
    /// </summary>
    /// 
    public static void AreSame(object? expected, object? actual)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Asserts that two objects do not refer to the same instance.
    /// </summary>
    /// 
    public static void AreNotSame(object? notExpected, object? actual)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Asserts that an object is `null`.
    /// </summary>
    /// 
    public static void IsNull(object? actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that an object is not `null`.
    /// </summary>
    /// 
    public static void IsNotNull(object? actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a collection contains a specific item.
    /// </summary>
    /// 
    public static void Contains(object? item, ICollection<object> collection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a collection does not contain a specific item.
    /// </summary>
    /// 
    public static void DoesNotContain(object? item, ICollection<object> collection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a collection is empty.
    /// </summary>
    /// 
    public static void IsEmpty(ICollection<object> collection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a collection is not empty.
    /// </summary>
    /// 
    public static void IsNotEmpty(ICollection<object> collection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that one collection is a subset of another.
    /// </summary>
    /// 
    public static void IsSubsetOf(object subset, object superset)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that two strings are equal ignoring case.
    /// </summary>
    /// 
    public static void AreEqualIgnoringCase(string expected, string actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a string starts with a specified prefix.
    /// </summary>
    /// 
    public static void StartsWith(string prefix, string actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a string ends with a specified suffix.
    /// </summary>
    /// 
    public static void EndsWith(string suffix, string actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that a string contains a specified substring.
    /// </summary>
    /// 
    public static void ContainsSubstring(object substring, object actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is greater than `b`.
    /// </summary>
    /// 
    public static void GreaterThan(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is greater than or equal to `b`.
    /// </summary>
    /// 
    public static void GreaterThanOrEqual(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is less than `b`.
    /// </summary>
    /// 
    public static void LessThan(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is less than or equal to `b`.
    /// </summary>
    /// 
    public static void LessThanOrEqual(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that two floating-point numbers are approximately equal within a specified tolerance.
    /// </summary>
    /// 
    public static void ApproximatelyEqual(object expected, object actual, object tolerance)
    {
        throw new NotImplementedException();
    }
}
