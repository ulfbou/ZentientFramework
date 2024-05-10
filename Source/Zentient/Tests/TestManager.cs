using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zentient.Extensions;

namespace Zentient.Tests;

public class TestManager
{
    //private Dictionary<Type, MethodInfo>  _testMethods = new Dictionary<Type, MethodInfo>();
    //private Dictionary<Type, MethodInfo> _testSetup = new Dictionary<Type, MethodInfo>();
    private Dictionary<Type, TestInfo> _testInfo = new Dictionary<Type, TestInfo>();

    public async Task Run()
    {
        if (_testInfo is null || _testInfo.Count() == 0) await LoadTests();

        foreach (var kvp in _testInfo!)
        {
            TestInfo testInfo = kvp.Value;
            try
            {
                testInfo.Setup?.Invoke(testInfo.Instance, new object[] { });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Failure when setting up: {ex.Message}");
                continue;
            }

            foreach (var test in testInfo.Tests)
            {
                try
                {
                    if (IsAsyncMethod(test))
                    {
                        await TesterAsync(kvp.Key, testInfo.Instance, test);
                    }
                    else
                    {
                        Tester(kvp.Key, testInfo.Instance, test);
                    }
                }
                catch(FailedTestException ex)
                {
                    await Console.Out.WriteLineAsync($"Testing {kvp.Key}: Failed! Reason: {ex.Message}.");
                }
                catch(Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Testing {kvp.Key}: Failed! Reason: {ex.Message}.");
                }
            }
        }
    }

    private async Task LoadTests()
    {
        await Console.Out.WriteLineAsync("Initializing tests.");

        // Get all types in the executing assembly.
        IAsyncEnumerable<Type> types = GetTestTypes();

        // Iterate over all types in executing assembly.
        await foreach (var type in types)
        {
            var methods = type.GetMethods();

            MethodInfo? setup = null;
            List<MethodInfo> tests = new List<MethodInfo>();

            foreach (var method in methods)
            {
                // Check if the method has SetupMethodAttribute
                if (method.GetCustomAttribute<TestSetupAttribute>() != null)
                {
                    if (setup != null)
                        throw new BadSetupException($"{type.FullName}: Multiple Methods annotated by TestSetupAttribute.");
                    setup = method;
                }
                // Check if the method has TestMethodAttribute
                else if (method.GetCustomAttribute<TestMethodAttribute>() != null)
                {
                    tests.Add(method);
                }
            }

            if (setup is null && tests.Count == 0)
                continue;

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

            _testInfo.Add(type, new TestInfo(instance!, setup, tests));
        }
    }

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

    internal static async Task TesterAsync(Type type, object instance, MethodInfo method)
    {
        try
        {
            await (Task)method.Invoke(instance, new object[] { })!;
            await Console.Out.WriteLineAsync($"Successfully tested {type.Name}.{method.Name}");
        }
        catch (FailedTestException ex)
        {
            await Console.Out.WriteLineAsync($"Testing {type.Name}.{method.Name}: Failed. Reason: {ex.Message}");
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Bad test in {type.Name}.{method.Name}: {ex.Message}");
            throw;
        }
    }

    internal static void Tester(Type type, object instance, MethodInfo method)
    {
        try
        {
            method.Invoke(instance, new object[] { });
            Console.Out.WriteLine($"Successfully tested {type.FullName}.");
        }
        catch (FailedTestException ex)
        {
            Console.Out.WriteLine($"Testing {type.Name}.{method.Name}: Failed. Reason: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine($"Bad test in {type.Name}.{method.Name}: {ex.Message}");
            throw;
        }
    }
    
    // Method to check if a MethodInfo represents an async method
    private static bool IsAsyncMethod(MethodInfo method)
    {
        return typeof(Task).IsAssignableFrom(method.ReturnType);
    }
}
