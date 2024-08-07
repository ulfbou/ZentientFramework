// Generate professional level comment documentation for the MockUtilities class. The class is a utility class that provides methods for creating, configuring, and verifying mock objects using the Moq library. The class is designed to be generic and flexible, allowing it to work with any type of mock object. The class is typically used in unit tests to create mock objects that simulate the behavior of real objects, making it easier to test code that depends on external dependencies. The class provides a set of methods for setting up mock behavior, verifying that mock behavior occurred, and creating mock objects of a specific type. The class is designed to be easy to use and understand, making it a valuable tool for developers who are writing unit tests for their code.
//
// Class: MockUtilities
//
// Description:
// The MockUtilities class is a utility class that provides methods for creating, configuring, and verifying mock objects using the Moq library. The class is designed to be generic and flexible, allowing it to work with any type of mock object. The class is typically used in unit tests to create mock objects that simulate the behavior of real objects, making it easier to test code that depends on external dependencies. The class provides a set of methods for setting up mock behavior, verifying that mock behavior occurred, and creating mock objects of a specific type. The class is designed to be easy to use and understand, making it a valuable tool for developers who are writing unit tests for their code. 
//
// Purpose:
// The purpose of the MockUtilities class is to provide a set of utility methods for working with mock objects in unit tests. The class is designed to be generic and flexible, allowing it to work with any type of mock object. The class provides methods for creating mock objects, setting up mock behavior, verifying that mock behavior occurred, and creating mock objects of a specific type. By using the MockUtilities class, developers can write unit tests that are more modular, flexible, and maintainable, leading to a more robust and scalable application. 
//
// Usage:
// The MockUtilities class is typically used in unit tests to create mock objects that simulate the behavior of real objects. The class provides a set of methods for creating mock objects, setting up mock behavior, verifying that mock behavior occurred, and creating mock objects of a specific type. By using the MockUtilities class, developers can write unit tests that are more modular, flexible, and maintainable, leading to a more robust and scalable application. 
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

using Moq;
using System.Dynamic;
using System.Linq.Expressions;

namespace Zentient.Tests.Mock
{
    public interface IMockUtilities
    {
        Mock<T> CreateMock<T>() where T : class;
        T CreateMockInstance<T>() where T : class, new();
        IEnumerable<MockCallInfo> GetCallHistory<T>(Mock<T> mock, Expression<Action<T>> expression) where T : class;
        Mock<T> GetMock<T>() where T : class;
        Mock<T> GetMock<T>(Expression<Action<T>> setupExpression) where T : class;
        T GetMockInstance<T>() where T : class;
        T GetMockInstance<T>(Expression<Action<T>> setupExpression) where T : class;
        T SetupAndVerifyBehavior<T>(Mock<T> mock, Expression<Action<T>> expression, Func<MockCallInfo, object> callback, Times times = default) where T : class;
        T SetupMockBehavior<T>(Mock<T> mock, Expression<Action<T>> expression) where T : class;
        T SetupMockBehavior<T>(Mock<T> mock, Expression<Action<T>> expression, Func<CallInfo, object> callback) where T : class;
        T SetupMockBehavior<T>(Mock<T> mock, Expression<Func<T, object>> expression, object value) where T : class;
        void SetupSequence<T>(Mock<T> mock, IEnumerable<Expression<Action<T>>> expressions) where T : class;
        void VerifyMockBehavior<T>(Mock<T> mock, Expression<Action<T>> expression, Times? times = null) where T : class;
    }
}