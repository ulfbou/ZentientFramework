//
// Class: ResponseMessage.cs
//
// Description:
// The <see cref="ResponseMessage"/> class is a concrete implementation of the <see cref="IResponseMessage"/> interface that can be used to create response messages that communicate the result of an operation to the caller. The class provides properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The class is generic, allowing the response message to include a data payload of any type. The class also provides static methods for creating successful and failed response messages, as well as methods for adding messages, warnings, and metadata to the response message. By using the <see cref="ResponseMessage"/> class, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The class is designed to be used in conjunction with the <see cref="IResponseMessage"/> interface, which defines the contract for a response message that can be used to communicate the result of an operation to the caller. By using the <see cref="ResponseMessage"/> class and the <see cref="IResponseMessage"/> interface, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class is part of the Zentient.Core.Utilities namespace, which contains utility classes and helper methods that can be used to improve the quality and maintainability of software applications. 
//
// Purpose:
// - Create a concrete implementation of the IResponseMessage interface that can be used to create response messages that communicate the result of an operation to the caller.
// - Provide properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation.
// - Allow the response message to include a data payload of any type.
// - Provide static methods for creating successful and failed response messages, as well as methods for adding messages, warnings, and metadata to the response message.
// - Create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation.
// - Write code that is more modular, flexible, and maintainable, leading to higher-quality software.
//
// Usage:
// - Use the ResponseMessage class to create concrete instances of the response message.
// - Use the static Ok, Fail and Create methods to create successful and failed response messages, respectively.
// - Use the StatusCode parameter to specify the HTTP status code for the response message.
// - Use the AddMessage, AddWarning, and AddMetadata methods to add information to the response message.
// - Use the Success, StatusCode, ErrorDetails, Messages, Warnings, and Metadata properties to access information about the response message.
// - Use the IResponseMessage interface to define the contract for a response message that can be used to communicate the result of an operation to the caller.
// - Use the IResponseMessage interface in conjunction with the ResponseMessage class to create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation.
// - Use the Zentient.Core.Utilities namespace to access the ResponseMessage class and other utility classes and helper methods that can be used to improve the quality and maintainability of software applications.
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

namespace Zentient.Core.Utilities
{
    /// <summary>
    /// The <see cref="ResponseMessage"/> class is a concrete implementation of the <see cref="IResponseMessage"/> interface that can be used to create response messages that communicate the result of an operation to the caller. The class provides properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The class is generic, allowing the response message to include a data payload of any type. The class also provides static methods for creating successful and failed response messages, as well as methods for adding messages, warnings, and metadata to the response message. By using the <see cref="ResponseMessage"/> class, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The class is designed to be used in conjunction with the <see cref="IResponseMessage"/> interface, which defines the contract for a response message that can be used to communicate the result of an operation to the caller. By using the <see cref="ResponseMessage"/> class and the <see cref="IResponseMessage"/> interface, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class is part of the Zentient.Core.Utilities namespace, which contains utility classes and helper methods that can be used to improve the quality and maintainability of software applications. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ResponseMessage"/> class is a concrete implementation of the <see cref="IResponseMessage"/> interface that can be used to create response messages that communicate the result of an operation to the caller. The class provides properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The class is generic, allowing the response message to include a data payload of any type. The class also provides static methods for creating successful and failed response messages, as well as methods for adding messages, warnings, and metadata to the response message. By using the <see cref="ResponseMessage"/> class, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The class is designed to be used in conjunction with the <see cref="IResponseMessage"/> interface, which defines the contract for a response message that can be used to communicate the result of an operation to the caller. By using the <see cref="ResponseMessage"/> class and the <see cref="IResponseMessage"/> interface, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class is part of the Zentient.Core.Utilities namespace, which contains utility classes and helper methods that can be used to improve the quality and maintainability of software applications.
    /// </para>
    /// </remarks>

    public class ResponseMessage
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ConcurrentBag<string> Messages { get; private set; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> Warnings { get; private set; } = new ConcurrentBag<string>();
        public ConcurrentDictionary<string, object> Metadata { get; private set; } = new ConcurrentDictionary<string, object>();

        public string? ErrorDetails { get; protected set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        protected ResponseMessage(HttpStatusCode statusCode, IEnumerable<string>? messages = null)
        {
            StatusCode = statusCode;
            if (messages is not null)
            {
                Messages = new ConcurrentBag<string>(messages);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the response message indicates success.
        /// </summary>
        public bool Success => StatusCode >= HttpStatusCode.OK && StatusCode < HttpStatusCode.Ambiguous;

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage"/> class with the specified status code and messages.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage"/> class with the specified status code and messages.</returns>
        public static ResponseMessage Ok(HttpStatusCode statusCode = HttpStatusCode.OK, IEnumerable<string>? messages = null)
            => new ResponseMessage(statusCode, messages);

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage"/> class with the specified status code and response message.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="responseMessage">The response message to include in the new instance.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage"/> class with the specified status code and response message.</returns>
        public static ResponseMessage Create(HttpStatusCode statusCode, IResponseMessage responseMessage)
        {
            ArgumentNullException.ThrowIfNull(responseMessage, nameof(responseMessage));
            var response = new ResponseMessage(statusCode, responseMessage.Messages)
            {
                ErrorDetails = responseMessage.ErrorDetails,
                Metadata = responseMessage.Metadata,
                Warnings = responseMessage.Warnings,
            };
            return response;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage"/> class with the specified status code and messages.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        /// <param name="errorDetails">The error details to include in the response message.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage"/> class with the specified status code and messages.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the messages parameter is null.</exception>
        public static ResponseMessage Fail(HttpStatusCode statusCode, IEnumerable<string>? messages = null, string? errorDetails = null)
        {
            ArgumentNullException.ThrowIfNull(messages, nameof(messages));
            var response = new ResponseMessage(statusCode, messages) { ErrorDetails = errorDetails };
            return response;
        }

        /// <summary>
        /// Adds a message to the response message.
        /// </summary>
        /// <param name="message">The message to add to the response message.</param>
        /// <returns>The response message with the added message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the message parameter is null.</exception>
        public ResponseMessage AddMessage(string message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            Messages.Add(message);
            return this;
        }

        /// <summary>
        /// Adds a warning to the response message.
        /// </summary>
        /// <param name="warning">The warning to add to the response message.</param>
        /// <returns>The response message with the added warning.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the warning parameter is null.</exception>
        public ResponseMessage AddWarning(string warning)
        {
            ArgumentNullException.ThrowIfNull(warning, nameof(warning));
            Warnings.Add(warning);
            return this;
        }

        /// <summary>
        /// Adds metadata to the response message.
        /// </summary>
        /// <param name="key">The key of the metadata to add to the response message.</param>
        /// <param name="value">The value of the metadata to add to the response message.</param>
        /// <returns>The response message with the added metadata.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the key or value parameter is null.</exception>
        public ResponseMessage AddMetadata(string key, object value)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            Metadata.TryAdd(key, value);
            return this;
        }
    }

    /// <summary>
    /// The <see cref="ResponseMessage{TData}"/> class is a generic implementation of the <see cref="ResponseMessage"/> class that can be used to create response messages that include a data payload of any type. The class provides a property for accessing the data payload, as well as properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The class is designed
    /// to be used in conjunction with the <see cref="IResponseMessage"/> interface, which defines the contract for a response message that can be used to communicate the result of an operation to the caller. By using the <see cref="ResponseMessage{TData}"/> class and the <see cref="IResponseMessage"/> interface, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The class is part of the Zentient.Core.Utilities namespace, which contains utility classes and helper methods that can be used to improve the quality and maintainability of software applications.
    /// </summary>
    /// <typeparam name="TData">The type of the data payload included in the response message.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="ResponseMessage{TData}"/> class is a generic implementation of the <see cref="ResponseMessage"/> class that can be used to create response messages that include a data payload of any type. The class provides a property for accessing the data payload, as well as properties and methods for setting and retrieving information about the status of the operation, any error details, messages, warnings, and metadata associated with the operation. The class is designed
    /// to be used in conjunction with the <see cref="IResponseMessage"/> interface, which defines the contract for a response message that can be used to communicate the result of an operation to the caller. By using the <see cref="ResponseMessage{TData}"/> class and the <see cref="IResponseMessage"/> interface, developers can create consistent and reusable response messages that provide valuable information to the caller about the outcome of an operation. The class is part of the Zentient.Core.Utilities namespace, which contains utility classes and helper methods that can be used to improve the quality and maintainability of software applications.
    /// </para>
    /// </remarks>
    public class ResponseMessage<TData> : ResponseMessage
    {
        public TData? Data { get; private set; }

        protected ResponseMessage(HttpStatusCode statusCode, TData? data = default, IEnumerable<string>? messages = null)
            : base(statusCode, messages)
        {
            Data = data;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and messages.
        /// </summary>
        /// <param name="data">The data payload to include in the response message.</param>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and messages.</returns>
        public static ResponseMessage<TData> Ok(TData? data, HttpStatusCode statusCode = HttpStatusCode.OK, IEnumerable<string>? messages = null)
        {
            return new ResponseMessage<TData>(statusCode, data, messages);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and messages.
        /// </summary>
        /// <param name="data">The data payload to include in the response message.</param>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and messages.</returns>
        public static new ResponseMessage<TData> Fail(HttpStatusCode statusCode, IEnumerable<string>? messages = null, string? errorDetails = null)
        {
            var response = new ResponseMessage<TData>(statusCode, default, messages) { ErrorDetails = errorDetails };
            return response;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and response message.
        /// </summary>
        /// <param name="data">The data payload to include in the response message.</param>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="messages">The messages to include in the response message.</param>
        /// <returns>A new instance of the <see cref="ResponseMessage{TData}"/> class with the specified status code, data, and messages.</returns>
        public static ResponseMessage<TData> Create(HttpStatusCode statusCode, TData data, IResponseMessage responseMessage)
        {
            var response = new ResponseMessage<TData>(statusCode, data, responseMessage.Messages)
            {
                ErrorDetails = responseMessage.ErrorDetails,
            };

            foreach (var item in responseMessage.Metadata)
            {
                response.Metadata.TryAdd(item.Key, item.Value);
            }

            foreach (var warning in responseMessage.Warnings)
            {
                response.Warnings.Add(warning);
            }

            return response;
        }
    }
}
