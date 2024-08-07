//
// Class: ExceptionHandler
//
// Description:
// The <see cref="ExceptionHandler"/> class represents a delegate that handles exceptions. It provides a set of methods for handling exceptions in a consistent and reusable way, including providing a cancellation token for asynchronous operations. The class is designed to be generic and flexible, allowing it to work with any exception type that implements the <see cref="Exception"/> class. The class also provides a way to handle exceptions in a more modular, flexible, and maintainable way, leading to higher-quality software. The class helps to decouple the exception handling logic from the rest of the application, making it easier to test and refactor the code in the future. 
//
// Purpose:
// The purpose of the <see cref="ExceptionHandler"/> class is to provide a common set of methods for handling exceptions in a consistent and reusable way. By defining a standard set of operations for handling exceptions, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the exception handling logic from the rest of the application, making it easier to test and refactor the code in the future. 
//
// Usage:
// The <see cref="ExceptionHandler"/> class is typically used in conjunction with asynchronous operations to handle exceptions in a consistent and reusable way. Developers can create concrete implementations of the <see cref="ExceptionHandler"/> class for specific exception types, providing a consistent and reusable way to handle exceptions. By using the exception handling pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application. 
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

namespace Zentient.Core.Helpers
{
    /// <summary>
    /// Definition for a delegate that handles exceptions.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <param name="cancellation">Optional. The cancellation token.</param>
    public delegate Task<Func<Exception, Task>> ExceptionHandler(Exception ex, CancellationToken cancellation = default);
}
