//
// Class: Assert
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
//t
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

public sealed class Assert
{
    // Implementing Singleton pattern
    public static Assert Instance { get; } = new Assert();

    private Assert() { }

    /// <summary>
    /// Validates that the test is true.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <param name="message">The message to be issued if the test fails.</param>
    /// <exception cref="AssertionFailureException">Thrown if the test fails.</exception>
    public static void Pass(bool test, string message = "")
    {
        if (!test) throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Validates that the provided function executes successfully without throwing any exceptions.
    /// </summary>
    /// <param name="action">The function to be validated.</param>
    /// <param name="message">The message to be issued if the test fails.</param>
    /// <exception cref="AssertionFailureException">Thrown if the test succeeds.</exception>
    public static void Pass(Func<object> action, string message = "")
    {
        // Act & Assert
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException(message, ex);
        }
    }

    /// <summary>
    /// Validates that the test is false.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <param name="message">The message to be issued if the test fails.</param>
    /// <exception cref="AssertionFailureException">Thrown if the test succeeds.</exception>
    public static void Fail(bool test, string message = "")
    {
        if (test) throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Validates that the provided action fails by throwing an exception.
    /// </summary>
    /// <param name="action">The action to be validated.</param>
    /// <exception cref="AssertionFailureException">Thrown if the test succeeds.</exception>
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

    /// <summary>
    /// Throws <see cref="AssertionFailureException"/>
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="AssertionFailureException">Thrown to mark the test as failed.</exception>
    public static void Fail(string message = "")
    {
        throw new AssertionFailureException(message);
    }
}
