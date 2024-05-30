//
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

namespace Zentient.Tests;

// Define the IObservable interface
public interface IObservable
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    void NotifyObservers(Exception ex);
}

public interface IObservableAsync
{
    Task Subscribe(IObserverAsync observer);
    Task Unsubscribe(IObserverAsync observer);
    Task NotifyObservers(Exception ex);
}
