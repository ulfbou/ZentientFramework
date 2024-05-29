//
// File: Assert.cs
//
// Description:
// The Assert class provides a set of static methods for making assertions in unit tests.These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria.
// 
// Usage:
// The Assert class is commonly used within unit testing frameworks such as NUnit and MSTest to verify the behavior of code under test. Developers use these assertion methods to validate various aspects of the code's output, behavior, and state during testing.
// 
// Purpose:
// The purpose of the Assert class is to provide a convenient and expressive way for developers to write unit tests and make assertions about the behavior and output of their code. By using these assertion methods, developers can ensure that their code behaves as expected under different conditions and scenarios, leading to more robust and reliable software.
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

namespace Zentient.Tests.Deprecated;
public static partial class ZTAssert
{
    public static void IsTrue(bool check)
    {
        //if (check) throw new FailedTestException($"Failed test: Expected true condition, but got false.");
    }

    public static void IsFalse(bool check)
    {
        //if (!check) throw new FailedTestException($"Failed test: Expected false condition, but got true.");
    }

    /// <summary>
    /// Asserts that an object is `null`.
    /// </summary>
    /// 
    public static void IsNull(object? actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that an object is not `null`.
    /// </summary>
    /// 
    public static void IsNotNull(object? actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that one collection is a subset of another.
    /// </summary>
    /// 
    public static void IsSubsetOf(object subset, object superset)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is greater than `b`.
    /// </summary>
    /// 
    public static void GreaterThan(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is greater than or equal to `b`.
    /// </summary>
    /// 
    public static void GreaterThanOrEqual(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is less than `b`.
    /// </summary>
    /// 
    public static void LessThan(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that `a` is less than or equal to `b`.
    /// </summary>
    /// 
    public static void LessThanOrEqual(object a, object b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that two floating-point numbers are approximately equal within a specified tolerance.
    /// </summary>
    /// 
    public static void ApproximatelyEqual(object expected, object actual, object tolerance)
    {
        throw new NotImplementedException();
    }
}
