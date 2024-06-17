//
// Class: AuthResponseDto
//
// Description:
// The AuthResponseDto class represents a data transfer object that is used to return authentication information to the client. The class contains properties for the authentication token and its expiration date, allowing the client to authenticate with the server and access protected resources. The class is designed to be simple and lightweight, providing only the information that is necessary for the client to authenticate successfully.
//
// Purpose:
// The purpose of the AuthResponseDto class is to provide a standardized way to return authentication information to the client. By encapsulating the authentication token and its expiration date in a single object, the class simplifies the process of authenticating with the server and accessing protected resources. The class helps to decouple the authentication logic from the rest of the application, making it easier to test and refactor the code in the future.
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

namespace Zentient.IdentityManagement.DTO
{
    /// <summary>
    /// Represents the response data transfer object for authentication.
    /// </summary>
    public class AuthResponseDto
    {
        public string JwtToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
