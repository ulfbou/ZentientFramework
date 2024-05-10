//
// File: TestInfo.cs
//
// Description:
// The TestInfo struct represents information about tests, including the test instance, setup method, and test methods associated with a test class. It encapsulates these details into a single structure for convenient storage and retrieval during test execution.
//
// Usage:
// Developers typically use the TestInfo struct within test management systems or frameworks to organize and manage tests. It allows them to store information about tests, such as the test instance and associated methods, in a structured format, facilitating the execution and reporting of test results.
//
// Purpose:
// The purpose of the TestInfo struct is to provide a lightweight and efficient way to represent and store information about tests within a test suite or framework. By encapsulating test details into a single structure, developers can simplify test management tasks and improve the organization and execution of tests within their applications.
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

using System.Reflection;

namespace Zentient.Tests;

/// <summary>
/// Represents information about tests including the test instance, setup method, and test methods.
/// </summary>
internal struct TestInfo
{
    private readonly object _instance;
    private readonly MethodInfo? _setup;
    private readonly IEnumerable<MethodInfo> _tests;

    /// <summary>
    /// Initializes a new instance of the TestInfo struct.
    /// </summary>
    /// <param name="instance">The instance of the test class.</param>
    /// <param name="setup">The setup method of the test class.</param>
    /// <param name="tests">The collection of test methods.</param>
    internal TestInfo(object instance, MethodInfo? setup, IEnumerable<MethodInfo> tests)
    {
        _instance = instance;
        _setup = setup;
        _tests = tests;
    }

    /// <summary>
    /// Gets the instance of the test class.
    /// </summary>
    internal object Instance { get => _instance; }

    /// <summary>
    /// Gets the setup method of the test class.
    /// </summary>
    internal MethodInfo? Setup { get => _setup; }

    /// <summary>
    /// Gets the collection of test methods.
    /// </summary>
    internal IEnumerable<MethodInfo> Tests { get => _tests; }
}
