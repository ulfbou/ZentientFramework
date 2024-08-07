//
// Class: AutoMapper
//
// Description:
// The AutoMapper class provides a way to map objects from one type to another. The class allows users to define mappings between different types of objects, specifying how the properties of one object should be copied to another object. The class is designed to be simple and lightweight, providing only the functionality that is necessary to map objects between different types.
//
// Purpose:
// The purpose of the AutoMapper class is to provide a standardized way to map objects between different types. By encapsulating the logic for mapping objects in a single class, the class simplifies the process of working with object mappings and ensures that the necessary functionality is provided. The class helps to decouple the object mapping logic from the rest of the application, making it easier to test and refactor the code in the future.
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

using AutoMapper;
using Zentient.IdentityManagement.DTO;
using Zentient.IdentityManagement.Identity;
using Zentient.IdentityManagement.Models;

namespace Zentient.IdentityManagement.Utilities
{
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<CreateUserDto, ApplicationUser>();

            // Role Mappings
            CreateMap<ApplicationRole, RoleDto>();
            CreateMap<CreateRoleDto, ApplicationRole>();

            // Privilege Mappings
            CreateMap<Privilege, PrivilegeDto>();
            CreateMap<CreatePrivilegeDto, Privilege>();
        }
    }
}
