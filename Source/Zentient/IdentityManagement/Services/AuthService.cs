//
// Class: AuthService
//
// Description:
// The AuthService class provides methods for authenticating and registering users with the server. The class contains methods for logging in and registering users, as well as generating JWT tokens for authenticated users. The class is designed to be simple and lightweight, providing only the functionality that is necessary to authenticate and register users with the server.
//
// Purpose:
// The purpose of the AuthService class is to provide a centralized service for authenticating and registering users with the server. By encapsulating the authentication and registration logic in a single class, the class simplifies the process of working with user authentication and registration and ensures that the necessary functionality is provided. The class helps to decouple the authentication and registration logic from the rest of the application, making it easier to test and refactor the code in the future.
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

using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Zentient.IdentityManagement.DTO;
using Zentient.IdentityManagement.Identity;
using Zentient.IdentityManagement.Models;
using Zentient.Repository;

namespace Zentient.IdentityManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public int MAX_FAILED_ATTEMPTS { get; private set; }

        public AuthService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        /// <summary>
        /// Authenticates a user with the server using the provided login credentials.
        /// </summary>
        /// <param name="loginDto">The login credentials of the user.</param>
        /// <returns>An authentication response containing a JWT token and expiration date.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the loginDto is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the email or password is invalid.</exception>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            ArgumentNullException.ThrowIfNull(loginDto, nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken };
        }

        /// <summary>
        /// Registers a new user with the server using the provided registration details.
        /// </summary>
        /// <param name="registerDto">The registration details of the user.</param>
        /// <returns>An authentication response containing a JWT token and expiration date.</returns>
        /// <exception cref="ArgumentException">Thrown when the registration details are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when user registration fails.</exception>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.Select(e => e.Description);
                var errorMessage = string.Join("; ", errorMessages);
                throw new InvalidOperationException($"User registration failed: {errorMessage}");
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken };
        }

        /// <summary>
        /// Refreshes the JWT token of an authenticated user using the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token of the user.</param>
        /// <returns>An authentication response containing a new JWT token and refresh token.</returns>
        /// <exception cref="SecurityException">Thrown when the refresh token is invalid.</exception>
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new SecurityException("Invalid refresh token");
            }

            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshTokenString();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto { JwtToken = newJwtToken, RefreshToken = newRefreshToken };
        }

        /// <summary>
        /// Revokes the refresh token of an authenticated user.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception name="SecurityTokenException">Thrown when the refresh token is invalid.</exception>
        public async Task RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                user.FailedTokenAttempts++;
                await _userManager.UpdateAsync(user);

                if (user.FailedTokenAttempts >= MAX_FAILED_ATTEMPTS)
                {
                    // TODO: Alert the user and/or take other action
                }

                throw new SecurityException("Invalid refresh token");
            }

            user.FailedTokenAttempts = 0;
            await _userManager.UpdateAsync(user);
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            refreshToken.IsRevoked = true;

            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Refreshes the JWT token of an authenticated user using the provided refresh token and rotates the refresh token.
        /// </summary>
        /// <param name="token">The refresh token of the user.</param>
        /// <returns>An authentication response containing a new JWT token and refresh token.</returns>
        /// <exception cref="SecurityTokenException">Thrown when the refresh token is invalid.</exception>
        public async Task<AuthResponseDto> RefreshTokenWithRotationAsync(string token)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) throw new SecurityTokenException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Token has expired or been revoked");
            }

            // Generate new tokens
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshTokenObject();

            // Revoke old refresh token and add new refresh token
            refreshToken.IsRevoked = true;
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                JwtToken = newJwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Invalid user ID `{userId}`");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to change password");
            }
        }

        public async Task LogoutAllSessionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            user.RefreshTokens.Clear();
            await _userManager.UpdateAsync(user);
        }

        public async Task LockAccountAsync(string userId, DateTimeOffset? dateTimeOffset = null)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            await _userManager.SetLockoutEndDateAsync(user, dateTimeOffset);
        }

        public async Task UnlockAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(-1));
        }

        public async Task<string?> GetVerificationCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Invalid user ID `{userId}`");
            }

            var verificationCode = GenerateVerificationCode(32);

            user.VerificationCode = verificationCode;
            await _userManager.UpdateAsync(user);

            return verificationCode;
        }

        public async Task VerifyCodeAndUnlockAccountAsync(string userId, string verificationCode)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Invalid user ID `{userId}`");
            }

            if (user.VerificationCode != verificationCode)
            {
                throw new InvalidOperationException("Invalid verification code");
            }

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(-1));
        }


        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private Zentient.IdentityManagement.Identity.RefreshToken GenerateRefreshTokenObject()
        {
            return new Zentient.IdentityManagement.Identity.RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateVerificationCode(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[4];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

    }
    /*
    If a refresh token is stolen and used by an attacker, they can potentially generate new JWT tokens and impersonate the user. To mitigate this risk, you can implement the following strategies:
    1.	Token revocation: Provide a way for users to revoke refresh tokens. This could be a user interface where users can see all of their active sessions (i.e., refresh tokens) and have the option to revoke any of them. When a token is revoked, remove it from the list of valid tokens in your database.
    2.	Short token lifespan: Make the refresh tokens short-lived. This reduces the window of opportunity for an attacker if they manage to steal a token.
    3.	Token rotation: Each time a client exchanges a refresh token for a new JWT token, also return a new refresh token and invalidate the old one. This ensures that a refresh token can only be used once, reducing the potential damage if a token is stolen.
    4.	Detect token abuse: If you see multiple failed attempts to use a refresh token, it could be a sign that the token has been stolen and someone is trying to use it. In this case, you could revoke the token and alert the user.
    5.	Secure token storage: On the client side, store tokens securely to reduce the chance of them being stolen. For web applications, this could mean storing tokens in HttpOnly cookies, which are not accessible to JavaScript.
    6.	Use HTTPS: Always use HTTPS for network communications to prevent tokens from being intercepted during transmission.
    Implementing these strategies can help you secure your application against refresh token theft. However, security is a complex field and it's important to stay up-to-date with best practices and continually review and improve your security measures. 
    */
}
