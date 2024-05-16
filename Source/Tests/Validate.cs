// File: Validate.cs
//
// Description:
// The Validate class provides methods for validating the success or failure of actions by combining an "Act" step with an assertion ("Assert"). This class is particularly useful for scenarios where the outcome of an action needs to be tested within a unit testing framework.
// 
// Usage:
// The Validate class offers two static methods: Fail and Success. 
//   - Fail: Accepts an Action delegate and verifies that the action throws an exception. If the action does not throw an exception, an assertion failure is triggered.
//   - Success: Accepts a Func<object> delegate and verifies that the action completes without throwing any exceptions. If an exception is thrown during the execution of the action, an assertion failure is triggered, displaying the exception message.
// 
// Purpose:
// The purpose of the Validate class is to simplify the process of testing the outcome of actions within unit tests. By encapsulating the "Act" step along with assertion logic, developers can easily verify whether an action behaves as expected under different conditions. The Fail method is useful for testing scenarios where an action is expected to fail, while the Success method is suitable for testing scenarios where an action is expected to succeed without throwing any exceptions. Using these methods, developers can enhance the robustness and reliability of their unit tests, ensuring that their code behaves correctly under various circumstances.
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

using Zentient.Tests;

namespace Tests;

internal class Validate
{
    /// <summary>
    /// Validates that the provided function executes successfully without throwing any exceptions.
    /// </summary>
    /// <param name="action">The function to be validated.</param>
    internal static void Pass(Func<object> action, string message = "")
    {
        // Act & Assert
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new AssertionFailureException(message, ex);
        }
    }

    /// <summary>
    /// Validates that the provided action fails by throwing an exception.
    /// </summary>
    /// <param name="action">The action to be validated.</param>
    internal static void Fail(Action action, string message = "")
    {
        // Act & Assert
        try
        {
            action();
        }
        catch
        {
            // Exception was thrown, considered as expected behavior
            return;
        }
        
        throw new AssertionFailureException(message);
    }
}
