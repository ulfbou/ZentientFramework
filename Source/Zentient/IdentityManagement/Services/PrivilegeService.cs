//
// Class: PrivilegeService
//
// Description:
// The PrivilegeService class provides methods for managing privileges in the system. The class contains methods for getting, creating, updating, and deleting privileges, allowing the system to define and manage different privileges for users. The class is designed to be simple and lightweight, providing only the functionality that is necessary to manage privileges in the system.
//
// Purpose:
// The purpose of the PrivilegeService class is to provide a standardized way to manage privileges in the system. By encapsulating the privilege management logic in a single class, the class simplifies the process of working with privileges and ensures that the necessary functionality is provided. The class helps to decouple the privilege management logic from the rest of the application, making it easier to test and refactor the code in the future.
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
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrivilegeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a privilege asynchronously by id.
        /// </summary>
        /// <param name="id">The id of the privilege to get.</param>
        /// <returns>The privilege with the specified id, or null if no privilege was found.</returns>
        public async Task<PrivilegeDto?> GetPrivilegeAsync(Guid id)
        {
            var privilege = await _unitOfWork.GetRepository<Privilege, Guid>().GetAsync(id);
            return _mapper.Map<PrivilegeDto>(privilege);
        }

        /// <summary>
        /// Get all privileges asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of all privileges in the system.</returns>
        public async Task<IEnumerable<PrivilegeDto>> GetAllPrivilegesAsync()
        {
            var privileges = await _unitOfWork.GetRepository<Privilege, Guid>().GetAllAsync();
            return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
        }

        /// <summary>
        /// Create a new privilege asynchronously.
        /// </summary>
        /// <param name="name">The name of the privilege.</param>
        /// <param name="description">The description of the privilege.</param>
        /// <returns>The newly created privilege.</returns>
        public async Task<PrivilegeDto> CreatePrivilegeAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException("Description cannot be null or empty.");

            var privilege = new Privilege
            {
                Name = name,
                Description = description
            };

            await _unitOfWork.GetRepository<Privilege, Guid>().AddAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrivilegeDto>(privilege);
        }

        /// <summary>
        /// Update an existing privilege asynchronously.
        /// </summary>
        /// <param name="id">The id of the privilege to update.</param>
        /// <param name="name">The new name of the privilege.</param>
        /// <param name="description">The new description of the privilege.</param>
        /// <returns>True if the privilege was updated successfully, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name or description is null or empty.</exception>
        public async Task<bool> UpdatePrivilegeAsync(Guid id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException("Description cannot be null or empty.");

            var privilegeRepo = _unitOfWork.GetRepository<Privilege, Guid>();
            var privilege = await privilegeRepo.GetAsync(id);

            if (privilege == null)
                return false;

            privilege.Name = name;
            privilege.Description = description;

            await privilegeRepo.UpdateAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete a privilege asynchronously.
        /// </summary>
        /// <param name="id">The id of the privilege to delete.</param>
        /// <returns>True if the privilege was deleted successfully, false otherwise.</returns>
        public async Task<bool> DeletePrivilegeAsync(Guid id)
        {
            var privilegeRepo = _unitOfWork.GetRepository<Privilege, Guid>();
            var privilege = await privilegeRepo.GetAsync(id);

            if (privilege == null)
                return false;

            await privilegeRepo.RemoveRangeAsync(new List<Privilege> { privilege });
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
