//
// File: TestAttributes.cs
// Description:
// This file contains custom attribute classes used for defining and organizing tests within test classes. These attributes are designed to mark classes and methods as test-related entities, allowing for the identification and execution of test cases in the test management system.
//
// Usage:
// These attributes are used to annotate classes and methods within a test suite, enabling developers to structure and organize their test cases effectively. Test management frameworks and tools can leverage these attributes to discover, execute, and report on test cases during automated testing processes.
//
// Purpose:
// The purpose of these attributes is to provide a standardized and declarative way to define test-related entities within the codebase. By annotating classes and methods with these attributes, developers can clearly indicate which components are intended for testing, facilitating automated test discovery and execution.
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
/// Specifies that a class contains test methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ZTTestClassAttribute : Attribute { }

/// <summary>
/// Specifies that a method is a test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ZTTestMethodAttribute : Attribute { }

/// <summary>
/// Specifies that a method is a setup method for tests within a test class.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ZTTestSetupAttribute : Attribute { }
