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
    /// <summary>
    /// Creates an assertion builder for the specified subject.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    /// <param name="subject">The subject to be asserted.</param>
    /// <returns>An instance of <see cref="IAssertionBuilder{T}"/>.</returns>
    public static IAssertionBuilder<T> That<T>(T subject) where T : class
        => new AssertionBuilder<T>(subject);

    /// <summary>
    /// Creates an exception assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="action">The action that may throw an exception.</param>
    /// <returns>An instance of <see cref="IExceptionAssertionBuilder"/>.</returns>
    public static IExceptionAssertionBuilder That(Action action)
        => new ExceptionAssertionBuilder(action, new AssertionBuilder<Action>(action));

    /// <summary>
    /// Creates an exception assertion builder for the specified action with a custom name, 
    /// allowing combined assertions with another assertion builder.
    /// </summary>
    /// <param name="action">The action that may throw an exception.</param>
    /// <param name="builder">The assertion builder for additional assertions.</param>
    /// <returns>An instance of <see cref="IExceptionAssertionBuilder"/>.</returns>
    public static IExceptionAssertionBuilder That(Action action, IAssertionBuilder<Action>? builder = null)
        => new ExceptionAssertionBuilder(action, builder ?? new AssertionBuilder<Action>(action));
}
