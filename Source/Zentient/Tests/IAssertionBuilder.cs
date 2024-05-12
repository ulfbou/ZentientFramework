﻿// TODO
// File: IAssertionBuilder.cs
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

namespace Zentient.Tests;

public interface IAssertionBuilder<T> where T : class
{
    IAssertionBuilder<T> IsEqualTo(T expectedValue);
    IAssertionBuilder<T> IsNotEqualTo(T expectedValue);
    IAssertionBuilder<T> IsNull();
    IAssertionBuilder<T> IsNotNull();
}

public interface IAssertionBuilder
{
    IAssertionBuilder IsEqualTo(object expectedValue);
    IAssertionBuilder IsNotEqualTo(object expectedValue);
    IAssertionBuilder IsNull();
    IAssertionBuilder IsNotNull();
}
