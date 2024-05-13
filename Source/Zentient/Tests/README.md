# Zentient.Tests

Zentient.Tests is a testing framework designed to facilitate unit testing in C# applications. It provides a set of utilities and conventions for writing, organizing, and executing unit tests.

## Features

- **Fluent API**: Zentient.Tests offers a fluent API for writing expressive and readable test assertions.
- **Test Setup**: Supports setup methods to prepare test environments before executing test cases.
- **Async Support**: Supports asynchronous test methods for testing asynchronous code.
- **Error Handling**: Provides robust error handling mechanisms for handling test failures and exceptions.
- **Test Discovery**: Automatically discovers and loads test classes and methods from the executing assembly.
- **Parallel Execution**: Supports parallel execution of tests to improve performance.

## Getting Started

To get started with Zentient.Tests, follow these steps:

1. **Installation**: Zentient.Tests can be installed via NuGet Package Manager.

   ```bash
   Install-Package Zentient.Tests
   ```

2. **Writing Tests**: Write unit tests using the fluent API provided by Zentient.Tests.

   ```csharp
   [TestMethod]
   public void MyTestMethod()
   {
       // Arrange
       var subject = new MyClass();

       // Act & Assert
       Zentient.Tests.Assert.That(subject).IsNotNull();
   }
   ```

3. **Running Tests**: Instantiate the `TestManager` class and invoke the `Run` method to execute all tests.

   ```csharp
   var testManager = new TestManager();
   await testManager.Run();
   ```

4. **Viewing Results**: View test results in the console output or integrate with a test runner for more detailed reporting.
