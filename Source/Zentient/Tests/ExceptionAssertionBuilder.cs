//
// Class: ExceptionAssertionBuilder
//
// Description:
// Provides fluent assertion methods for validating thrown exceptions and other conditions. These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria. The AssertionBuilder class facilitates a more readable and maintainable approach to writing tests by enabling a chainable method syntax.
// 
// Usage:
// The ExceptionAssertionBuilder class facilitates unit testing assertions on object states. Upon instantiation with the target test object, it offers a sequence of chainable methods designed to throw exceptions upon assertion failure, thereby indicating test failures.
// 
// Purpose:
// The ExceptionAssertionBuilder class aims to provide a straightforward and articulate means for crafting test assertions. Leveraging a fluent interface pattern, it simplifies test code structure and enhances the clarity of test assertions, aiding developers in grasping the test's purpose and the standards for its success.
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

/// <summary>
/// Provides fluent assertion methods for validating thrown exceptions and other conditions.
/// </summary>
public class ExceptionAssertionBuilder(Action action, IAssertionBuilder<Action>? builder = null)
    : IExceptionAssertionBuilder
{
    private readonly Action _action = action;
    private readonly IAssertionBuilder<Action> _builder = builder ?? new AssertionBuilder<Action>(action);
    private string? _thrownMessage = null;

    /// <summary>
    /// Gets the underlying assertion builder associated with this exception assertion.
    /// </summary>
    public IAssertionBuilder<Action> Builder { get { return _builder; } }

    /// <summary>
    /// Asserts that the specified delegate throws an exception of type <typeparamref name="TException"/> or its derived types.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    public IExceptionAssertionBuilder Throws<TException>() where TException : Exception
    {
        try
        {
            _action();
        }
        catch (TException ex)
        {
            _thrownMessage = ex.Message;
            return this;
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate throws exception <see cref="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    public IExceptionAssertionBuilder ThrowsExactly<TException>() where TException : Exception
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            var type = ex.GetType();

            if (type == typeof(TException))
            {
                _thrownMessage = ex.Message;
                return this;
            }

            throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but {type.Name} was thrown instead.");
        }

        throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate throws an exception that inherits from <see cref="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    public IExceptionAssertionBuilder ThrowsDerived<TException>() where TException : Exception
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            var type = ex.GetType();

            if (type != typeof(TException) && type.IsSubclassOf(typeof(TException)))
            {
                _thrownMessage = ex.Message;
                return this;
            }

            throw new AssertionFailureException($"Expected an exception inherited from `{GetTypeName<TException>()}` to be thrown, but {type.Name} was thrown instead.");
        }

        throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate does throw any exception. 
    /// </summary>
    /// <param name="value">The operation to be executed.</param>
    public IExceptionAssertionBuilder ThrowsAny()
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            _thrownMessage = ex.Message;
            return this;
        }

        throw new AssertionFailureException($"Expected an exception to be thrown, but no exception was thrown.");
    }

    /// <summary>
    /// Asserts that the specified delegate neither throws exception <see cref="TException"/> nor its derived types.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    public IExceptionAssertionBuilder DoesNotThrow<TException>() where TException : Exception
    {
        try
        {
            _action();
            _thrownMessage = string.Empty;
            return this;
        }
        catch (TException ex)
        {
            throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }
        catch (Exception ex)
        {
            _thrownMessage = ex.Message;
            return this;
        }
    }

    /// <summary>
    /// Asserts that the specified delegate does not throw exception <see cref="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected not to be thrown.</typeparam>
    public IExceptionAssertionBuilder DoesNotThrowExactly<TException>() where TException : Exception
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            var type = ex.GetType();

            if (type != typeof(TException))
            {
                _thrownMessage = ex.Message;
                return this;
            }

            throw new AssertionFailureException($"Expected `{GetTypeName<TException>()}` to be thrown, but {type.Name} was thrown instead.");
        }

        _thrownMessage = string.Empty;
        return this;
    }

    /// <summary>
    /// Asserts that the specified delegate does not throw an exception that inherits from <see cref="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    public IExceptionAssertionBuilder DoesNotThrowDerived<TException>() where TException : Exception
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            var type = ex.GetType();

            if (type == typeof(TException) || !type.IsSubclassOf(typeof(TException)))
            {
                _thrownMessage = ex.Message;
                return this;
            }

            throw new AssertionFailureException($"Expected an exception not inherited from `{GetTypeName<TException>()}` to be thrown, but {type.Name} was actually thrown.");
        }

        _thrownMessage = string.Empty;
        return this;
    }

    /// <summary>
    /// Asserts that the specified delegate does not throw any exception. 
    /// </summary>
    /// <param name="value">The operation to be executed.</param>
    public IExceptionAssertionBuilder DoesNotThrowAny()
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException($"Expected no exceptions to be thrown, but `{ex.GetType().Name}` was actually thrown.");
        }

        _thrownMessage = string.Empty;
        return this;
    }

    /// <summary>
    /// Asserts that a delegate does not throw an exception.
    /// </summary>
    /// 
    public IExceptionAssertionBuilder DoesNotThrow()
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException($"Throws {ex.GetType().Name} when expecting no exception to be thrown.", ex);
        }

        _thrownMessage = string.Empty;
        return this;
    }

    public IExceptionAssertionBuilder WithMessage(string expectedMessage)
    {
        if (_thrownMessage is null) throw new InvalidOperationException($"Expected exception message `{expectedMessage}`, but no exception was thrown.");

        if (_thrownMessage != expectedMessage) throw new AssertionFailureException();

        return this;
    }

    public IExceptionAssertionBuilder WithMessageContaining(string expectedMessage)
    {
        if (_thrownMessage is null) throw new InvalidOperationException($"Expected exception message `{expectedMessage}`, but no exception was thrown.");

        if (!_thrownMessage.Contains(expectedMessage)) throw new AssertionFailureException();

        return this;
    }

    #region async
#if async
    /// <summary>
    /// Asserts that a delegate throws exception <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
    /// <param name="value">The asynchronous operation to be executed.</param>
    public static async Task ThrowsAsync<T>(Func<Task<object>> value) where T : Exception
    {
        try
        {
            await value();
        }
        catch (T)
        {
            return this;
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException($"Expected `{GetTypeName<T>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new AssertionFailureException($"Expected `{GetTypeName<T>()}` to be thrown, but no exception was thrown.");
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
            throw new AssertionFailureException($"Expected `{GetTypeName<T>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        throw new AssertionFailureException($"Expected `{GetTypeName<T>()}` to be thrown, but no exception was thrown.");
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
            throw new AssertionFailureException($"Threw {ex.GetType().Name} when it is expected to not be thrown.", ex);
        }
        catch (Exception) { }
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

        throw new AssertionFailureException($"Expected an exception to be thrown, but no exception was thrown.");
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
            throw new AssertionFailureException($"Throws {ex.GetType().Name} when expecting no exception to be thrown.", ex);
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
            throw new AssertionFailureException($"Expected {nameof(T)} to be thrown, but {ex.GetType().Name} was thrown instead.");
        }

        // If control reaches here, it means the expected exception T was not thrown
        throw new AssertionFailureException($"Failed test: Expected exception to be thrown, but it wasn't.");
    }

#endif
    #endregion

    private static string GetTypeName<T>() => typeof(T).Name;
}
