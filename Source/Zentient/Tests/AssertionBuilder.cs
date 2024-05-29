﻿//
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
public class AssertionBuilder<T> : AssertionBuilderBase<T>, IAssertionBuilder<T> where T : class
{
    protected readonly IComparer<T> _comparer;
    protected readonly IEqualityComparer<T> _equality;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="actual">The actual value to be asserted.</param>
    /// <param name="comparer">Optional. The comparer to be used for equality checks.</param>
    /// <param name="equality">Optional. The equality comparer to be used for equality checks.</param>
    /// <param name="message">Optional. The message to be issued if the assertion fails.</param>
    public AssertionBuilder(T actual, IComparer<T>? comparer = null, IEqualityComparer<T>? equality = null, string message = "")
        : base(actual, message)
    {
        _comparer = comparer ?? DefaultComparers<T>.Comparer;
        _equality = equality ?? DefaultComparers<T>.EqualityComparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    /// <param name="comparer">The comparer to be used for equality checks.</param>
    /// <param name="message">Optional. Custom error message.</param>
    public AssertionBuilder(T subject, IComparer<T> comparer, string message = "")
        : this(subject, comparer, null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="actual">The actual value to be asserted.</param>
    /// <param name="equality">The equality comparer to be used for equality checks.</param>
    /// <param name="message">Optional. The message to be issued if the assertion fails.</param>
    public AssertionBuilder(T actual, IEqualityComparer<T>? equality, string message = "")
        : this(actual, null, equality, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    /// <param name="message">Custom error message.</param>
    public AssertionBuilder(T subject, string message)
        : this(subject, null, null, message) { }

    #region AssertionBuilder 
    public override int Compare(T? actual, T? expected) => _comparer.Compare(_actual, expected);
    public override bool Equals(T? actual, T? expected) => _equality.Equals(actual, expected);
    public override bool Equals(object? expected) => _equality.Equals(_actual, (T?)expected);

    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public override IAssertionBuilder<T> IsEqualTo(T expected, string message = "Failed test")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(Equals(expected), message);

        return this;
    }

    public IAssertionBuilder<T> IsGreaterThan(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Assert.Pass(Compare(_actual, value) > 0);
        return this;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
    #endregion

}

/// <summary>
/// Provides fluent assertion methods for comparing equality and other conditions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
/// </remarks>
/// <param name="actual">The subject to be asserted.</param>
/// <param name="comparer">Optional. The comparer to be used for equality checks.</param>
/// <param name="equality">Optional. The equality comparer to be used for equality checks.</param>
/// <param name="message">Optional. The message to provide should the assertion fail.</param>
public class AssertionBuilder(
    object actual,
    IComparer<object>? comparer,
    IEqualityComparer<object>? equality,
    string message = "")
    : IAssertionBuilder
{
    private readonly object _actual = actual;
    private readonly IComparer<object> _comparer = comparer ?? DefaultComparers<object>.Comparer;
    private readonly IEqualityComparer<object> _equality = equality ?? DefaultComparers<object>.EqualityComparer;
    private readonly string _message = message;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="actual">The subject to be asserted.</param>
    /// <param name="message">Optional. Custom error message.</param>
    public AssertionBuilder(object actual, string message)
        : this(actual, null, null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="actual">The subject to be asserted.</param>
    /// <param name="comparer">The comparer to be used for equality checks.</param>
    /// <param name="message">Optional. Custom error message.</param>
    public AssertionBuilder(
        object actual,
        IComparer<object>? comparer,
        string message = "")
        : this(actual, comparer, null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="actual">The subject to be asserted.</param>
    /// <param name="equality">The equality comparer to be used for equality checks.</param>
    /// <param name="message">Optional. Custom error message.</param>
    public AssertionBuilder(object actual, IEqualityComparer<object> equality, string message = "")
        : this(actual, DefaultComparers<object>.Comparer, equality, message) { }

    public virtual int Compare(object? expected) => _comparer.Compare(_actual, expected);
    public virtual int Compare(object? actual, object? expected) => _comparer.Compare(actual, expected);
    public override bool Equals(object? expected) => _equality.Equals(_actual, expected);

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotNull(string message = "")
    {
        Assert.Fail(_actual is null, message);
        return this;
    }

    /// <summary>
    /// Asserts that the subject is true.
    /// </summary>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsTrue(string message = "")
    {
        if (_actual is bool isTrue)
        {
            if (isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Asserts that the subject is false.
    /// </summary>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsFalse(string message = "")
    {
        if (_actual is bool isTrue)
        {
            if (!isTrue) return this;
        }

        throw new AssertionFailureException(message);
    }

    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public virtual IAssertionBuilder IsEqualTo(object expected, string message = "Failed test")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Pass(Equals(_actual, expected), message);

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotEqualTo(object expected, string message = "")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(Equals(expected), message);

        return this;
    }

    /// <summary>
    /// Asserts that the subject is not the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotSameAs(object expected, string message = "")
    {
        ArgumentNullException.ThrowIfNull(expected);
        Assert.Fail(ReferenceEquals(_actual, expected), message);

        return this;
    }

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <param name="message">Optional. Custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNull(string message = "")
    {
        Assert.Pass(_actual is null, message);
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
        Assert.Pass(ReferenceEquals(_actual, expected), message);

        return this;
    }

    public override int GetHashCode() => base.GetHashCode();
}
