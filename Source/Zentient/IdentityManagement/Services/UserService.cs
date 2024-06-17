//
// Class: UserService
//
// Description:
// The UserService class provides methods for managing users in the system. The class allows users to create, update, and delete users, as well as retrieve information about specific users. The class is designed to be simple and lightweight, providing only the functionality that is necessary to manage users in the system.
//
// Purpose:
// The purpose of the UserService class is to provide a standardized way to manage users in the system. By encapsulating the logic for creating, updating, and deleting users in a single class, the class simplifies the process of working with users and ensures that the necessary functionality is provided. The class helps to decouple the user management logic from the rest of the application, making it easier to test and refactor the code in the future.
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
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Zentient.IdentityManagement.DTO;
using Zentient.IdentityManagement.Identity;
using Zentient.IdentityManagement.Models;
using Zentient.Repository;

namespace Zentient.IdentityManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a user asynchronously by id.
        /// </summary>
        /// <param name="id">The id of the user to get.</param>
        /// <returns>The user with the specified id, or null if no user was found.</returns>
        public async Task<UserDto?> GetUserAsync(Guid id)
        {
            var user = await _unitOfWork.GetRepository<User, Guid>().GetAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Get all users asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of all users in the system.</returns>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.GetRepository<User, Guid>().GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <summary>
        /// Create a new user asynchronously.
        /// </summary>
        /// <param name="dto">The registration details of the user.</param>
        /// <returns>The newly created user.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dto is null.</exception>
        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                UserRoles = dto.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList()
            };

            await _unitOfWork.GetRepository<User, Guid>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Update a user asynchronously.
        /// </summary>
        /// <param name="id">The id of the user to update.</param>
        /// <param name="dto">The registration details of the user.</param>
        /// <returns>True if the user was updated successfully, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dto is null.</exception>
        public async Task<bool> UpdateUserAsync(Guid id, CreateUserDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            var userRepo = _unitOfWork.GetRepository<User, Guid>();
            var user = await userRepo.GetAsync(id);

            if (user is null) return false;

            user.Username = dto.Username;
            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = HashPassword(dto.Password);
            }

            user.UserRoles = dto.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList();

            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete a user asynchronously.
        /// </summary>
        /// <param name="id">The id of the user to delete.</param>
        /// <returns>True if the user was deleted successfully, false otherwise.</returns>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var userRepo = _unitOfWork.GetRepository<User, Guid>();
            var user = await userRepo.GetAsync(id);

            if (user == null)
                return false;

            await userRepo.RemoveRangeAsync(new List<User> { user });
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(null!, password);
        }
    }
}
