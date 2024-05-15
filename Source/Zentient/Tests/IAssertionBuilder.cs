//
// Interface: IAssertBuilder
//
// Description:
// Defines a set of fluent assertion methods for evaluating equality, type compatibility, and other logical conditions. These methods empower developers to confirm the expected behavior and results of the code under test. The IAssertBuilder interface promotes a more intuitive and sustainable testing practice by supporting a chainable method syntax.
//
// Usage:
// The IAssertBuilder interface is implemented to facilitate unit testing assertions on object states. Upon instantiation with the target test object, it offers a sequence of chainable methods designed to throw exceptions upon assertion failure, thereby indicating test failures.
//
// Purpose:
// The IAssertBuilder interface aims to provide a straightforward and articulate means for crafting test assertions. Leveraging a fluent interface pattern, it simplifies test code structure and enhances the clarity of test assertions, aiding developers in grasping the test's purpose and the standards for its success.
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

public interface IAssertionBuilder<T>
{
    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsEqualTo(T expected, string message = "");

    /// <summary>
    /// Asserts that the subject is not equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotEqualTo(T expected, string message = "");

    /// <summary>
    /// Asserts that the subject is the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsSameAs(T expected, string message = "");

    /// <summary>
    /// Asserts that the subject is the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotSameAs(T expected, string message = "");

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNull(string message = "");

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder<T> IsNotNull(string message = "");
    
    public IAssertionBuilder<T> IsTrue(string message = "");
    void IsGreaterThan(int v);
}

public interface IAssertionBuilder
{
    /// <summary>
    /// Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    IAssertionBuilder IsEqualTo(object expected, string message = "");

    /// <summary>
    /// Asserts that the subject is not equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    IAssertionBuilder IsNotEqualTo(object expected, string message = "");

    /// <summary>
    /// Asserts that the subject is the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsSameAs(object expected, string message = "");

    /// <summary>
    /// Asserts that the subject is not the same instance as the expected object.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <exception cref="AssertFailedException">Thrown if the assertion fails.</exception>
    IAssertionBuilder IsNotSameAs(object expected, string message = "");

    /// <summary>
    /// Asserts that the subject is null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNull(string message = "");

    /// <summary>
    /// Asserts that the subject is not null.
    /// </summary>
    /// <exception cref="AssertionFailureException">Thrown if the assertion fails.</exception>
    public IAssertionBuilder IsNotNull(string message = "");
}
