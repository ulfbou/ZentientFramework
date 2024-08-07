//
// Class: StringAssertionBuilder
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

public class StringAssertionBuilder
    : AssertionBuilder<string>, IStringAssertionBuilder
{
    private readonly bool _isComparerIgnoringCase;
    private readonly bool _isEqualityComparerIgnoringCase;

    public StringAssertionBuilder(string actual, IComparer<string> comparer, IEqualityComparer<string> equality, string message)
        : base(actual, comparer, equality, message)
    {
        _isComparerIgnoringCase = DefaultComparers<string>.Comparer != comparer && IsComparerIgnoringCase();
        _isEqualityComparerIgnoringCase = DefaultComparers<string>.EqualityComparer != equality && IsEqualityComparerIgnoringCase();
    }

    public StringAssertionBuilder(string actual, IComparer<string> comparer, string message = "") :
        this(actual, comparer, DefaultComparers<string>.EqualityComparer, message)
    { }

    public StringAssertionBuilder(string actual, IEqualityComparer<string> equality, string message = "") :
        this(actual, DefaultComparers<string>.Comparer, equality, message)
    { }

    public StringAssertionBuilder(string actual, string message = "") :
        this(actual,
            DefaultComparers<string>.Comparer,
            DefaultComparers<string>.EqualityComparer,
            message)
    { }

    /// <summary>
    /// Asserts that two strings are equal ignoring case.
    /// </summary>
    /// 
    public IStringAssertionBuilder AreEqualsIgnoringCase(string expected)
    {
        ArgumentNullException.ThrowIfNull(expected);

        if (_isComparerIgnoringCase)
        {
            Assert.Pass(_comparer.Compare(_actual, expected) == 0, $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
            return this;
        }

        if (_isEqualityComparerIgnoringCase)
        {
            Assert.Pass(_equality.Equals(_actual, expected), $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
            return this;
        }

        Assert.Pass(string.Equals(_actual, expected, StringComparison.OrdinalIgnoreCase), $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
        return this;
    }

    public IStringAssertionBuilder AreNotEqualIgnoringCase(string expected)
    {
        ArgumentNullException.ThrowIfNull(expected);

        if (_isComparerIgnoringCase)
        {
            Assert.Pass(_comparer.Compare(_actual, expected) == 0, $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
            return this;
        }

        if (_isEqualityComparerIgnoringCase)
        {
            Assert.Pass(_equality.Equals(_actual, expected), $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
            return this;
        }

        Assert.Pass(string.Equals(_actual, expected, StringComparison.OrdinalIgnoreCase), $"Expected string `{expected}` does not match the actual string `{_actual}` ignoring case.");
        return this;
    }

    /// <summary>
    /// Asserts that a string starts with a specified prefix.
    /// </summary>
    /// 
    public IStringAssertionBuilder StartsWith(string prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        Assert.Pass(_actual.StartsWith(prefix, _comparer));
        return this;
    }

    /// <summary>
    /// Asserts that a string ends with a specified suffix.
    /// </summary>
    public IStringAssertionBuilder EndsWith(string suffix)
    {
        ArgumentNullException.ThrowIfNull(suffix);
        Assert.Pass(_actual.EndsWith(suffix, _comparer));
        return this;
    }

    /// <summary>
    /// Asserts that a string contains a specified substring.
    /// </summary>
    public IStringAssertionBuilder Contains(string substring)
    {
        ArgumentNullException.ThrowIfNull(substring);
        Assert.Pass(_actual.Contains(substring, _comparer));
        return this;
    }

    private bool IsComparerIgnoringCase()
    {
        if (_comparer == DefaultComparers<string>.Comparer) return false;
        var lowerActual = _actual.ToLowerInvariant();
        var upperActual = _actual.ToUpperInvariant();

        return _comparer.Compare(lowerActual, _actual) == 0 &&
               _comparer.Compare(upperActual, _actual) != 0;
    }

    private bool IsEqualityComparerIgnoringCase()
    {
        if (_comparer == DefaultComparers<string>.EqualityComparer) return false;
        var lowerActual = _actual.ToLowerInvariant();
        var upperActual = _actual.ToUpperInvariant();

        return _equality.Equals(_actual, lowerActual) &&
               !_equality.Equals(_actual, upperActual);
    }
}