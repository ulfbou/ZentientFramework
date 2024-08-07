﻿//
// File: EqualityAsserts.cs
//
// Description:
// A partial implementation of the Assert class that validates equality testing. The Assert class provides a set of static methods for making assertions in unit tests.These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria.
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
    public static void AreEqual(object? expectedObject, object? actualObject)
    {
        AreEqual<object>(expectedObject, actualObject);
    }

    public static void AreEqual<T>(T expectedObject, T actualObject)
    {
        ArgumentNullException.ThrowIfNull(expectedObject);
        ArgumentNullException.ThrowIfNull(actualObject);

        if (!expectedObject.Equals(actualObject))
        {
            throw new FailedTestException($"Failed test: Expected {expectedObject}, but got {actualObject}.");
        }
    }

    public static void AreNotEqual(object? notExpectedObject, object actualObject)
    {
        AreNotEqual<object>(notExpectedObject, actualObject);
    }

    public static void AreNotEqual<T>(T notExpectedObject, T actualObject)
    {
        try
        {
            AreEqual(notExpectedObject, actualObject);
        }
        catch (FailedTestException ex)
        {
            return;
        }
        catch { }
        throw new FailedTestException($"Failed test: Expected to not receive {notExpectedObject}, but got {actualObject}.");
    }

    public static void ReferenceEquals<T>(T expectedObject, T actualObject)
    {
        if (!object.ReferenceEquals(expectedObject, actualObject))
        {
            throw new FailedTestException($"Expected two objects to share references.");
        }
    }

    /// <summary>
    /// Asserts that two objects refer to the same instance.
    /// </summary>
    /// 
    public static void AreSame(object? expected, object? actual)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asserts that two objects do not refer to the same instance.
    /// </summary>
    /// 
    public static void AreNotSame(object? notExpected, object? actual)
    {
        throw new NotImplementedException();
    }

    internal static string GetReferenceString<T>(T? obj) where T : class
    {
        return obj != null ? $"({obj.GetType().Name}@{obj.GetHashCode()})" : "null";
    }
}

