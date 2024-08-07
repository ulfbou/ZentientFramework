//
// Class: RoleService
//
// Description:
// The RoleService class provides methods for managing roles in the system. The class allows users to create, update, and delete roles, as well as retrieve information about specific roles. The class is designed to be simple and lightweight, providing only the functionality that is necessary to manage roles in the system.
//
// Purpose:
// The purpose of the RoleService class is to provide a standardized way to manage roles in the system. By encapsulating the logic for creating, updating, and deleting roles in a single class, the class simplifies the process of working with roles and ensures that the necessary functionality is provided. The class helps to decouple the role management logic from the rest of the application, making it easier to test and refactor the code in the future.
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
using Zentient.IdentityManagement.Models;
using Zentient.Repository;

namespace Zentient.IdentityManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a role asynchronously by id.
        /// </summary>
        /// <param name="id">The id of the role to get.</param>
        /// <returns>The role with the specified id, or null if no role was found.</returns>
        public async Task<RoleDto?> GetRoleAsync(Guid id)
        {
            var role = await _unitOfWork.GetRepository<Role, Guid>().GetAsync(id);
            return _mapper.Map<RoleDto>(role);
        }

        /// <summary>
        /// Get all roles asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of all roles in the system.</returns>
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.GetRepository<Role, Guid>().GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        /// <summary>
        /// Create a new role asynchronously.
        /// </summary>
        /// <param name="dto">The registration details of the Role.</param>
        /// <returns>The newly created role.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dto is null.</exception>
        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                RolePrivileges = dto.PrivilegeIds.Select(privilegeId => new RolePrivilege { PrivilegeId = privilegeId }).ToList()
            };

            await _unitOfWork.GetRepository<Role, Guid>().AddAsync(role);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<RoleDto>(role);
        }

        /// <summary>
        /// Update a role asynchronously.
        /// </summary>
        /// <param name="id">The id of the role to update.</param>
        /// <param name="dto">The registration details of the role.</param>
        /// <returns>True if the role was updated successfully, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dto is null.</exception>
        public async Task<bool> UpdateRoleAsync(Guid id, CreateRoleDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            var roleRepo = _unitOfWork.GetRepository<Role, Guid>();
            var role = await roleRepo.GetAsync(id);

            if (role == null)
                return false;

            role.Name = dto.Name;
            role.Description = dto.Description;
            role.RolePrivileges = dto.PrivilegeIds.Select(privilegeId => new RolePrivilege { PrivilegeId = privilegeId }).ToList();

            await roleRepo.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete a role asynchronously.
        /// </summary>
        /// <param name="id">The id of the role to delete.</param>
        /// <returns>True if the role was deleted successfully, false otherwise.</returns>
        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var roleRepo = _unitOfWork.GetRepository<Role, Guid>();
            var role = await roleRepo.GetAsync(id);

            if (role == null)
                return false;

            await roleRepo.RemoveRangeAsync(new List<Role> { role });
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
