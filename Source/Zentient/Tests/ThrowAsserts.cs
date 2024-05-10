//
// File: ThrowAsserts.cs
//
// Description:
// A partial implementation of the Assert class that validates exception testing. The Assert class provides a set of static methods for making assertions in unit tests.These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria.
// 
// Usage:
// The Assert class is commonly used within unit testing frameworks such as NUnit and MSTest to verify the behavior of code under test. Developers use these assertion methods to validate various aspects of the code's output, behavior, and state during testing.
// 
// Purpose:
// The purpose of the Assert class is to provide a convenient and expressive way for developers to write unit tests and make assertions about the behavior and output of their code. By using these assertion methods, developers can ensure that their code behaves as expected under different conditions and scenarios, leading to more robust and reliable software.
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

using System.Dynamic;

namespace Zentient.Tests;

public static partial class Assert 
{
    /// <summary>
    /// Asserts that the specified delegate throws exception <see cref="T">exception</see>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The operation to be executed.</param>
    public static void Throws<T>(Func<object> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            value();
        }
        catch (T)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but no exception was thrown.");
    }

   // Asserts that the specified asynchronous operation throws an exception of the specified type.
    /// <summary>
    /// Asserts that a delegate throws exception <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The asynchronous operation to be executed.</param>
    public static async Task ThrowsAsync<T>(Func<Task<object>> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            await value();
        }
        catch (T)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate throws exception <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The asynchronous operation to be executed.</param>
    public static async Task Throws<T>(Func<Task> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            await value();
        }
        catch (T)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new FailedTestException($"Failed test: Expected `{GetTypeName<T>()}` to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate does not throw exception <see cref="T">exception</see>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The operation to be executed.</param>
    public static void DoesNotThrow<T>(Func<object> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            value();
        }
        catch (T ex)
        {
            throw new FailedTestException($"Failed test: Threw {ex.GetType().Name} when it is expected to not be thrown.", ex);
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Asserts that the specified asynchronous delegate does not throw exception <see cref="T">exception</see>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The asynchronous operation to be executed.</param>
    public static async Task DoesNotThrowAsync<T>(Func<Task<object>> value) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            await value();
        }
        catch (T ex)
        {
            throw new FailedTestException($"Failed test: Threw {ex.GetType().Name} when it is expected to not be thrown.", ex);
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Asserts that the specified asynchronous delegate does not throw an exception
    /// </summary>
    /// <param name="value">The operation to be executed.</param>
    public static void ThrowsAny(Func<object> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            value();
        }
        catch
        {
            return;
        }

        throw new FailedTestException($"Failed test: Expected an exception to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified asynchronous delegate does not throw an exception of any type.
    /// </summary>
    /// <param name="value">The asynchronous operation to be executed.</param>
    public static async Task ThrowsAnyAsync(Func<Task<object>> value)
    {
        ArgumentNullException.ThrowIfNull(nameof(value));

        try
        {
            await value();
        }
        catch
        {
            return;
        }

        throw new FailedTestException($"Failed test: Expected an exception to be thrown, but no exception was thrown.");
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

    private static object GetTypeName<T>() where T : Exception
    {
        T? instance = CreateInstance<T>();
        throw new NotImplementedException();
    }

    private static T? CreateInstance<T>() where T : Exception
    {
        T? instance = null;

        try
        {
            instance = Activator.CreateInstance(typeof(T)) as T;
        }
        catch
        {}

        return instance;
    }
}
