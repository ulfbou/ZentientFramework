//
// Interface: IRoleService
//
// Description:
// The IRoleService interface defines the contract for a service that provides role management functionality in the system. The interface contains methods for getting, creating, updating, and deleting roles, allowing the client to manage roles in the system. The interface is designed to be simple and lightweight, providing only the methods that are necessary for managing roles in the system.
//
// Purpose:
// The purpose of the IRoleService interface is to provide a standardized way to manage roles in the system. By defining a common interface for role management functionality, the interface simplifies the process of working with roles and ensures that the necessary functionality is provided. The interface helps to decouple the role management logic from the rest of the application, making it easier to test and refactor the code in the future.
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

using Zentient.IdentityManagement.DTO;

namespace Zentient.IdentityManagement.Services
{
    public interface IRoleService
    {
        Task<RoleDto?> GetRoleAsync(Guid id);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
        Task<bool> UpdateRoleAsync(Guid id, CreateRoleDto dto);
        Task<bool> DeleteRoleAsync(Guid id);
    }
}
