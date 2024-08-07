//
// Class: ServiceCollectionExtensions
//
// Description:
// The ServiceCollectionExtensions class provides extension methods for the IServiceCollection interface that simplify the process of configuring the Identity Management services in an ASP.NET Core application. The class contains methods for adding the necessary services to the service collection, including the database context, Identity services, and JWT authentication. The class is designed to be simple and lightweight, providing only the functionality that is necessary to configure the Identity Management services in an ASP.NET Core application. The class helps to decouple the configuration logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Purpose:
// The purpose of the ServiceCollectionExtensions class is to provide a standardized way to configure the Identity Management services in an ASP.NET Core application. By encapsulating the configuration logic in a single class, the class simplifies the process of setting up the necessary services and ensures that the correct services are added to the service collection. The class helps to decouple the configuration logic from the rest of the application, making it easier to test and refactor the code in the future.
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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zentient.IdentityManagement.Identity;
using Zentient.IdentityManagement.Services;
using Zentient.Repository;

namespace Zentient.IdentityManagement
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityManagement<TContext, TUser, TRole>(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IdentityOptions> identityOptions = null,
            Action<DbContextOptionsBuilder> dbContextOptions = null,
            Action<JwtBearerOptions> jwtOptions = null)
            where TContext : DbContext
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            // Configure database context
            services.AddDbContext<TContext>(dbContextOptions ?? (options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
                    throw new InvalidOperationException("No connection string found in configuration."))));

            services.AddScoped(typeof(IRepository<,>), typeof(CachedRepositoryBase<,>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            // Add Identity services
            var identityBuilder = services.AddIdentity<TUser, TRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

            if (identityOptions != null)
            {
                services.Configure(identityOptions);
            }

            // Add JWT Authentication
            services.AddJwtAuthentication(configuration, jwtOptions);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add AuthService
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions> jwtOptions = null)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                jwtOptions?.Invoke(options);
            });

            return services;
        }
    }
}
