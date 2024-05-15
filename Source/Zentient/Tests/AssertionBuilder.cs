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
public partial class AssertionBuilder<T>(T subject, IComparer<T> comparer, IEqualityComparer<T> equality, string message)
    : IAssertionBuilder<T>
{
    private readonly T _subject = subject;
    private readonly IComparer<T> _comparer = comparer;
    private readonly IEqualityComparer<T> _equality = equality;
    private readonly string _message = message;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(T subject, string message = "")
        : this(subject, DefaultComparers<T>.Comparer, DefaultComparers<T>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(T subject, IComparer<T> comparer, string message = "")
        : this(subject, comparer, DefaultComparers<T>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(T subject, IEqualityComparer<T> equality, string message = "")
        : this(subject, DefaultComparers<T>.Comparer, equality, message) { }

    public IComparer<T> Comparer => _comparer;
    public IEqualityComparer<T> EqualityComparer => _equality;

    public override bool Equals(object? obj) => _equality.Equals(_subject, (T?)obj);
    public override int GetHashCode() => base.GetHashCode();
    public int Compare(object? obj) => Comparer.Compare(_subject, (T?)obj);

    #region AssertionBuilder 
    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsEqualTo(T expected, string message = "Failed test")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(Equals(expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(Equals(expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(ReferenceEquals(_subject, expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(ReferenceEquals(_subject, expected), message);

        return this;
    }

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNull(string message = "")
    {
        Assert.Pass(_subject is null, message);
        return this;
    }

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotNull(string message = "")
    {
        Assert.Fail(_subject is null, message);
        return this;
    }

    /// <summary>
    /// Asserts that the subject is true.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsTrue(string message = "")
    {
        if (_subject is bool isTrue)
        {
            if (isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Asserts that the subject is false.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsFalse(string message = "")
    {
        if (_subject is bool isTrue)
        {
            if (!isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }

    public void IsGreaterThan(int v)
    {
        throw new NotImplementedException();
    }
    #endregion

}

/// <summary>
/// Provides fluent assertion methods for comparing equality and other conditions.
/// </summary>
public class AssertionBuilder : IAssertionBuilder
{
    private readonly object _subject;
    private readonly IComparer<object> _comparer;
    private readonly IEqualityComparer<object> _equality;
    private readonly string _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(object subject, string message = "")
        : this(subject, DefaultComparers<object>.Comparer, DefaultComparers<object>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(object subject, IComparer<object> comparer, string message = "")
        : this(subject, comparer, DefaultComparers<object>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public AssertionBuilder(object subject, IEqualityComparer<object> equality, string message = "")
        : this(subject, DefaultComparers<object>.Comparer, equality, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    /// <param name="message">The message to provide should the assertion fail.</param>
    public AssertionBuilder(object subject, IComparer<object> comparer, IEqualityComparer<object> equality, string message)
    {
        _subject = subject;
        _comparer = comparer;
        _equality = equality;
        _message = message;
    }

    public IComparer<object> Comparer { get; }
    public IEqualityComparer<object> EqualityComparer { get; }

    public override bool Equals(object? obj) => EqualityComparer.Equals(_subject, obj);
    public override int GetHashCode() => base.GetHashCode();
    public int Compare(object? obj) => Comparer.Compare(_subject, obj);

    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsEqualTo(object expected, string message = "")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(Equals(expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(Equals(expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(ReferenceEquals(_subject, expected), message);

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
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(ReferenceEquals(_subject, expected), message);

        return this;
    }

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNull(string message = "")
    {
        Assert.Pass(_subject is null, message);
        return this;
    }

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotNull(string message = "")
    {
        Assert.Fail(_subject is null, message);
        return this;
    }

    /// <summary>
    /// Asserts that the subject is true.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsTrue(string message = "")
    {
        if (_subject is bool isTrue)
        {
            if (isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Asserts that the subject is false.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsFalse(string message = "")
    {
        if (_subject is bool isTrue)
        {
            if (!isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }
}
