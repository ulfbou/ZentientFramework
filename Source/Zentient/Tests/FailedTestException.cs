// File: FailedTestException.cs
//
// Description:
// The FailedTestException is thrown when an assertion in a test method fails. This exception indicates that the expected outcome of a test case did not match the actual result, signaling a failure in the test scenario.
//
// Usage:
// This exception is commonly used within testing frameworks and test suites to signal test failures. When thrown, it provides information about the reason for the failure, enabling developers to diagnose and address issues in their test cases.
//
// Purpose:
// The FailedTestException class serves as a specialized exception type for handling test failures. By throwing this exception when an assertion fails, developers can gracefully handle test failures and capture relevant information about the failure, aiding in debugging and resolution of issues in test scenarios.
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
/// Exception thrown when the Assert in a test method fails.
/// </summary>
public class FailedTestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTestException"/> class.
    /// </summary>
    public FailedTestException() {}

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public FailedTestException(string? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTestException"/> class with a specified error message.
    /// and a reference to the inner exception that caused this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public FailedTestException(string? message, Exception ex)
	: base(message, ex) {}
}
