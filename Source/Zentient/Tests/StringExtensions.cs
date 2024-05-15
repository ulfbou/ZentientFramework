//
// Class: StringExtensions
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

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string strA, string strB, StringComparer comparer)
    {
        return comparer.Equals(strA, strB);
    }

    public static bool EqualsIgnoreCase(this string strA, string strB, IEqualityComparer<string> comparer)
    {
        return comparer.Equals(strA, strB);
    }

    public static bool StartsWith(this string str, string prefix, IComparer<string> comparer)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(comparer));

        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(prefix))
        {
            return false;
        }

        if (prefix.Length > str.Length)
        {
            return false;
        }

        string substring = str.Substring(0, prefix.Length);
        return comparer.Compare(substring, prefix) == 0;
    }

    public static bool EndsWith(this string str, string suffix, IComparer<string> comparer)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(comparer));

        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(suffix))
        {
            return false;
        }

        if (suffix.Length > str.Length)
        {
            return false;
        }

        string substring = str.Substring(str.Length - suffix.Length);
        return comparer.Compare(substring, suffix) == 0;
    }

    public static bool Contains(this string str, string substring, IComparer<string> comparer)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(str));
        ArgumentNullException.ThrowIfNull(substring, nameof(substring));
        ArgumentNullException.ThrowIfNull(comparer, nameof(comparer));

        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substring))
        {
            return false;
        }

        return Enumerable.Range(0, str.Length - substring.Length + 1)
            .Any(i => comparer.Compare(str.Substring(i, substring.Length), substring) == 0);
    }
}
