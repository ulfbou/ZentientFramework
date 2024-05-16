//
// Class: AssertBuilder
// 
// Description:
// Defines a set of fluent assertion methods for evaluating equality, type compatibility, and other logical conditions. These methods empower developers to confirm the expected behavior and results of the code under test. The IAssertBuilder interface promotes a more intuitive and sustainable testing practice by supporting a chainable method syntax.
// 
// Usage:
// The AssertBuilder class facilitates unit testing assertions on object states. Upon instantiation with the target test object, it offers a sequence of chainable methods designed to throw exceptions upon assertion failure, thereby indicating test failures.
// 
// Purpose:
// The AssertBuilder class aims to provide a straightforward and articulate means for crafting test assertions. Leveraging a fluent interface pattern, it simplifies test code structure and enhances the clarity of test assertions, aiding developers in grasping the test's purpose and the standards for its success.
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
/// Provides fluent assertion methods for comparing equality and other conditions.
/// </summary>
/// <typeparam name="T">The type of the subject being asserted.</typeparam>
public class AssertionBuilder<T> : IAssertionBuilder<T> where T : class
{
    private readonly T _subject;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(T subject)
    {
        _subject = subject;
    }

    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsEqualTo(T expected, string message = "")
    {
        if (expected is null)
        {
            throw new ArgumentNullException(nameof(expected));
        }

        if (!_subject.Equals(expected))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotEqualTo(T expected, string message = "")
    {
        if (expected is null)
        {
            throw new ArgumentNullException(nameof(expected));
        }

        if (_subject.Equals(expected))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsSameAs(T expected, string message = "")
    {
        if (!ReferenceEquals(expected, _subject))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotSameAs(T expected, string message = "")
    {
        if (ReferenceEquals(expected, _subject))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNull()
    {
        if (_subject != null)
        {
            throw new AssertionFailureException();
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotNull()
    {
        if (_subject is null)
        {
            throw new AssertionFailureException();
        }

        return this;
    }
}

/// <summary>
/// Provides fluent assertion methods for comparing equality and other conditions.
/// </summary>
public class AssertionBuilder(object subject) : IAssertionBuilder
{
    private readonly object _subject = subject;

    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsEqualTo(object expected, string message = "")
    {
        if (expected is null) throw new ArgumentNullException(nameof(expected));

        if (!_subject.Equals(expected))
        {
            throw new AssertionFailureException();
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotEqualTo(object expected, string message = "")
    {
        if (expected is null) throw new ArgumentNullException(nameof(expected));

        if (_subject.Equals(expected))
        {
            throw new AssertionFailureException();
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsSameAs(object expected, string message = "")
    {
        if (!ReferenceEquals(expected, _subject))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotSameAs(object expected, string message = "")
    {
        if (ReferenceEquals(expected, _subject))
        {
            throw new AssertionFailureException(message);
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    IAssertionBuilder IAssertionBuilder.IsNull()
    {
        if (_subject != null)
        {
            throw new AssertionFailureException();
        }

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    IAssertionBuilder IAssertionBuilder.IsNotNull()
    {
        if (_subject is null)
        {
            throw new AssertionFailureException();
        }

        return this;
    }
}