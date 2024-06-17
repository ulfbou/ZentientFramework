//
// Class: LoginDto
//
// Description:
// The LoginDto class represents a data transfer object that is used to authenticate a user with the server. The class contains properties for the email and password of the user, allowing the client to provide the necessary credentials to authenticate successfully. The class is designed to be simple and lightweight, providing only the information that is necessary for the client to authenticate with the server.
//
// Purpose:
// The purpose of the LoginDto class is to provide a standardized way to authenticate a user with the server. By encapsulating the email and password of the user in a single object, the class simplifies the process of authenticating with the server and ensures that the necessary information is provided. The class helps to decouple the authentication logic from the rest of the application, making it easier to test and refactor the code in the future.
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
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
