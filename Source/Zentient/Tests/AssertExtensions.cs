//
// Class: AssertExtensions
//
// Description:
// The Assert class provides a fluent API with a set of static methods for making assertions in unit tests. These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria.
// 
// Usage:
// The Assert class is commonly used within unit testing frameworks to verify the behavior of code under test. Developers use these assertion methods, part of a fluent API, to validate various aspects of the code's output, behavior, and state during testing.
// 
// Purpose:
// The purpose of the Assert class is to provide a convenient and expressive way for developers to write unit tests and make assertions about the behavior and output of their code. By using these assertion methods, which are part of a fluent API, developers can chain together multiple assertions in a readable and concise manner, ensuring that their code behaves as expected under different conditions and scenarios, leading to more robust and reliable software.
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
#if false
{
    /// <summary>
    /// Creates an assertion builder for the specified subject.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    /// <param name="subject">The subject to be asserted.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder{T}"/>.</returns>
    public static IAssertionBuilder<T> That<T>(T subject)
        => new AssertionBuilder<T>(subject, string.Empty);

    /// <summary>
    /// Creates an exception assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="action">The action that may throw an exception.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder"/>.</returns>
    public static IAssertionBuilder That(Action action)
        => new AssertionBuilder(action, new AssertionBuilder<Action>(action));

    /// <summary>
    /// Creates an exception assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="action">The action that may throw an exception.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder"/>.</returns>
    public static IAssertionBuilder That(Action action, string message)
        => new AssertionBuilder(action, new AssertionBuilder<Action>(action, message), message);

    /// <summary>
    /// Creates an exception assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="action">The action that may throw an exception.</param>
    /// <param name="builder">The assertion builder for additional assertions.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder"/>.</returns>
    public static IAssertionBuilder That(
        Action action, 
        IAssertionBuilder<Action>? builder, 
        string message)
        => new AssertionBuilder(action, builder ?? new AssertionBuilder<Action>(action, message), message);

    /// <summary>
    /// Creates a collection assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="collection">The collection to be asserted.</param>
    /// <param name="builder">The assertion builder for additional assertions.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder"/>.</returns>
    public static ICollectionAssertionBuilder<T> That<T>(
        ICollection<T> collection) where T:ICollection<T>
        => new CollectionAssertionBuilder<T>(collection, new AssertionBuilder<ICollection<T>>(collection), string.Empty);

    /// <summary>
    /// Validates that the provided function executes successfully without throwing any exceptions.
    /// </summary>
    /// <param name="action">The function to be validated.</param>
    public static void Pass(Func<object> action, string message = "")
    {
        // Act & Assert
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates that the provided action fails by throwing an exception.
    /// </summary>
    /// <param name="action">The action to be validated.</param>
    public static void Fail(Action action, string message = "")
    {
        // Act & Assert
        try
        {
            action();
        }
        catch
        {
            // Exception was thrown, considered as expected behavior
            return;
        }

        throw new AssertionFailureException(message);
    }
}
#endif

/// <summary>
/// Creates an exception assertion builder for the specified action with a custom name, 
/// allowing combined assertions with another assertion builder.
/// </summary>
/// <param name="action">The action that may throw an exception.</param>
/// <returns>An instance of <see cref="IAssertionBuilder"/>.</returns>

public static class AssertExtensions
{
    /// <summary>
    /// Creates an exception assertion builder for the specified action.
    /// </summary>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="action">The action that may throw an exception.</param>
    /// <returns>An instance of <see cref="IExceptionAssertionBuilder"/>.</returns>
    public static IExceptionAssertionBuilder That(this Assert assert, Action action)
        => new ExceptionAssertionBuilder(action);


    /// <summary>
    /// Creates a collection assertion builder for the specified array.
    /// </summary>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="array">The array to be asserted.</param>
    /// <returns>An instance of ICollectionAssertionBuilder.</returns>
    public static ICollectionAssertionBuilder<T> That<T>(this Assert assert, T[] array)
        => new CollectionAssertionBuilder<T>(array);

    /// <summary>
    /// Creates a collection assertion builder for the specified collection.
    /// </summary>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="collection">The collection to be asserted.</param>
    /// <returns>An instance of ICollectionAssertionBuilder.</returns>
    // var duplicateCollection = new List<int> { 1, 2, 2, 3 };
    // Assert.That<int>(duplicateCollection);
    public static ICollectionAssertionBuilder<T> That<T>(this Assert assert, ICollection<T> collection)
        => new CollectionAssertionBuilder<T>(collection);

    /// <summary>
    /// Creates a collection assertion builder for the specified enumerable.
    /// </summary>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="enumeration">The enumerable to be asserted.</param>
    /// <returns>An instance of ICollectionAssertionBuilder.</returns>
    // var duplicateCollection = new List<int> { 1, 2, 2, 3 };
    // Assert.That((IEnumerable<int>) duplicateCollection)
    // var expected = collection1.Zip(collection2, (x, y) => new Pair(x, x));
    // Assert.That<Pair>(actual);
    public static ICollectionAssertionBuilder<T> That<T>(this Assert assert, IEnumerable<T> enumeration)
        => new CollectionAssertionBuilder<T>(enumeration.ToList());

    /// <summary>
    /// Creates an assertion builder for the specified subject.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="subject">The subject to be asserted.</param>
    /// <returns>An instance of IAssertionBuilder.</returns>
    public static IAssertionBuilder<T> That<T>(this Assert assert, T subject)
        => new AssertionBuilder<T>(subject);

    /// <summary>
    /// Creates an assertion builder for the specified string message.
    /// </summary>
    /// <param name="assert">The assert object calling the extension method.</param>
    /// <param name="message">The message to be asserted.</param>
    /// <returns>An instance of IStringAssertionBuilder.</returns>
    public static IStringAssertionBuilder That(this Assert assert, string message)
        => new StringAssertionBuilder(message);
}
