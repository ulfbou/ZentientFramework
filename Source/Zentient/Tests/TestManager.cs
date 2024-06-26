﻿//
// Class: TestManager
//
// Description:
// The TestManager class is responsible for managing and executing tests defined within the executing assembly. It provides functionality to load tests, run tests, and handle test setup and execution.
// 
// Usage:
// The TestManager class serves as the entry point for running tests within an application or test suite. Developers typically instantiate an instance of this class and invoke the Run method to execute all tests defined within the assembly. Additionally, the class provides internal methods for loading tests, retrieving test types, and executing individual test methods asynchronously or synchronously.
// 
// Purpose:
// The purpose of the TestManager class is to provide a centralized component for managing and executing tests within an application or test suite. By encapsulating test execution logic within this class, developers can easily organize, execute, and report on tests, facilitating the process of test-driven development (TDD) and ensuring the reliability and correctness of their codebase.
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
/// Manages the execution of asynchronous and synchronous tests defined within test classes.
/// </summary>
public class TestManager
{
    private Dictionary<Type, TestInfo> _testInfo = new Dictionary<Type, TestInfo>();
    private bool _supress;

    /// <summary>
    /// Runs all tests asynchronously.
    /// </summary>
    /// <remarks>
    /// This method loads all tests, invokes setup methods if available, and executes each test method.
    /// </remarks>
    public async Task Run(bool supress = false)
    {
        _supress = supress;

        if (_testInfo is null || _testInfo.Count() == 0)
        {
            await LoadTests();
        }

        // Execute tests concurrently
        if (_testInfo != null)
        {
            await Task.WhenAll(_testInfo.Select(kvp => RunTestsAsync(kvp.Key, kvp.Value)));
        }
    }

    private async Task RunTestsAsync(Type testType, TestInfo testInfo)
    {
        int count = 0;
        int success = 0;

        try
        {
            testInfo.Setup?.Invoke(testInfo.Instance, new object[] { });
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Failure when setting up: {ex.Message}");
            return;
        }

        foreach (var test in testInfo.Tests)
        {
            try
            {
                count++;
                if (IsAsyncMethod(test))
                {
                    await TestAsync(testType, testInfo.Instance, test);
                }
                else
                {
                    Test(testType, testInfo.Instance, test);
                }
                success++;
            }
            catch (AssertionFailureException ex)
            {
                await Console.Out.WriteLineAsync($"Testing {testType.FullName}: Failed! Reason: {ex.Message}.");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Testing {testType.FullName}: Failed! Reason: {ex.Message}.");
            }
        }

        await Console.Out.WriteLineAsync($"Ran {success} successful tests out of {count} tests.");
    }

    /// <summary>
    /// Loads all test classes and their methods.
    /// </summary>
    /// <exception cref="BadSetupException">Thrown when there are issues with the setup of test classes.</exception>
    /// <remarks>
    /// This method searches for test classes in the executing assembly and collects their setup methods and test methods.
    /// </remarks>
    private async Task LoadTests()
    {
        await Console.Out.WriteLineAsync("Initializing tests.");

        // Retrieve test types asynchronously
        IAsyncEnumerable<Type> types = GetTestTypes();

        // Load tests concurrently
        await foreach (var type in types)
        {
            await Task.Run(async () =>
            {
                // Load and process each test type
                await LoadAndProcessTestType(type);
            });
        }
    }

    private async Task LoadAndProcessTestType(Type type)
    {
        await Console.Out.WriteLineAsync($"Loading {type.FullName}");
        // Load and process tests for the given type
        // Add appropriate error handling here
        var methods = type.GetMethods();

        MethodInfo? setup = null;
        List<MethodInfo> tests = new List<MethodInfo>();

        foreach (var method in methods)
        {
            // Check if the method has SetupMethodAttribute
            if (method.GetCustomAttribute<TestSetupAttribute>() != null)
            {
                if (setup != null)
                {
                    throw new BadSetupException($"{type.FullName}: Multiple Methods annotated by TestSetupAttribute.");
                }

                setup = method;
            }
            // Check if the method has TestMethodAttribute
            else if (method.GetCustomAttribute<TestMethodAttribute>() != null)
            {
                tests.Add(method);
            }
        }

        if (setup is null && tests.Count == 0)
            return;

        object? instance;
        try
        {
            instance = Activator.CreateInstance(type);

            if (instance is null)
                throw new BadSetupException($"Could not create an instance of {type.FullName}.");
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Error {type.FullName}: {ex.Message}");
            throw;
        }

        await Console.Out.WriteLineAsync($"Adding {type.FullName}");

        TestInfo value;

        if (!_testInfo.TryGetValue(type, out value))
        {
            _testInfo.Add(type, new TestInfo(instance!, setup, tests));
        }
        else
        {

        }
    }

    /// <summary>
    /// Retrieves all types marked with the TestClassAttribute from the executing assembly asynchronously.
    /// </summary>
    /// <returns>An asynchronous enumerable collection of types marked with the TestClassAttribute.</returns>
    private async IAsyncEnumerable<Type> GetTestTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assemblyTasks = assemblies
            .Select(assembly => Task.Run(() => assembly.GetTypes().Where(type => type.GetCustomAttribute<TestClassAttribute>() != null)));
        var typesLists = await Task.WhenAll(assemblyTasks);
        var types = typesLists.SelectMany(t => t);
        foreach (Type type in types)
        {
            yield return type;
        }
    }

    /// <summary>
    /// Executes an asynchronous test method of a specified type and handles exceptions.
    /// </summary>
    /// <param name="type">The type of the test class containing the method.</param>
    /// <param name="instance">An instance of the test class.</param>
    /// <param name="method">The MethodInfo representing the test method.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task TestAsync(Type type, object instance, MethodInfo method)
    {
        string name = $"{type.FullName}.{method.Name}";
        try
        {
            await (Task)method.Invoke(instance, new object[] { })!;
            await Console.Out.WriteLineAsync($"Successfully tested {name}");
        }
        catch (AssertionFailureException ex)
        {
            await Console.Out.WriteLineAsync($"Testing {name}: Failed. Reason: {ex.Message}");
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Bad test in {name}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Executes a synchronous test method of a specified type and handles exceptions.
    /// </summary>
    /// <param name="type">The type of the test class containing the method.</param>
    /// <param name="instance">An instance of the test class.</param>
    /// <param name="method">The MethodInfo representing the test method.</param>
    internal void Test(Type type, object instance, MethodInfo method)
    {
        string name = $"{type.FullName}.{method.Name}";
        try
        {
            method.Invoke(instance, new object[] { });
            if (!_supress)
            {
                Console.Out.WriteLine($"Successfully tested {name}.");
            }
        }
        catch (AssertionFailureException ex)
        {
            Console.Out.WriteLine($"Testing {name}: Failed. Reason: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine($"Bad test in {name}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Checks if a MethodInfo represents an asynchronous method.
    /// </summary>
    /// <param name="method">The MethodInfo to check.</param>
    /// <returns>True if the method is asynchronous; otherwise, false.</returns>
    private static bool IsAsyncMethod(MethodInfo method)
    {
        return typeof(Task).IsAssignableFrom(method.ReturnType);
    }
}
