//
// Class: IResponseMessage.cs
//
// Description:
// This interface defines the contract for a response message that can be used to communicate the result of an operation to the caller. The response message contains information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The interface is generic, allowing the response message to include a data payload of any type. The interface also provides methods for adding messages, warnings, and metadata to the response message, making it easy to customize the content of the message as needed. By using this interface, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The interface is designed to be used in conjunction with the ResponseMessage class, which implements the IResponseMessage interface and provides a concrete implementation of the response message functionality. By using the IResponseMessage interface, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software.
//
// Purpose:
// - Define the contract for a response message that can be used to communicate the result of an operation to the caller.
// - Include information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation.
// - Allow the response message to include a data payload of any type.
// - Provide methods for adding messages, warnings, and metadata to the response message.
// - Create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation.
// - Write code that is more modular, flexible, and maintainable, leading to higher-quality software.
//
// Usage:
// - Define a class that implements the IResponseMessage interface to create a custom response message.
// - Use the AddMessage, AddWarning, and AddMetadata methods to add information to the response message.
// - Use the Success, StatusCode, ErrorDetails, Messages, Warnings, and Metadata properties to access information about the response message.
// - Use the ResponseMessage class to create concrete instances of the response message.
// - Use the static Ok, Fail and Create methods to create successful and failed response messages, respectively. 
// - Use the StatusCode parameter to specify the HTTP status code for the response message.
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

using System.Collections.Concurrent;
using System.Net;
using Zentient.Core.Utilities;

public interface IResponseMessage
{
    string? ErrorDetails { get; }
    ConcurrentBag<string> Messages { get; }
    ConcurrentDictionary<string, object> Metadata { get; }
    HttpStatusCode StatusCode { get; }
    bool Success { get; }
    ConcurrentBag<string> Warnings { get; }

    ResponseMessage AddMessage(string message);
    ResponseMessage AddMetadata(string key, object value);
    ResponseMessage AddWarning(string warning);
}
