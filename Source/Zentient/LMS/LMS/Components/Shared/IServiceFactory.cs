using LMS.Core.Entities;
using LMS.Core.Identity;
using LMS.Core.Services;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;

using Zentient.Core.Services;

namespace LMS.Components.Shared
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICRUDService<TEntity> GetService<TEntity, TKey>() where TEntity : class, IEntity<TKey> where TKey : struct
        {
            return _serviceProvider.GetService<ICRUDService<TEntity>>();
        }

    }
    public interface IServiceFactory
    {
        public interface IServiceFactory
        {
            ICRUDService<TEntity> GetService<TEntity, TKey>()
                where TEntity : class, IEntity<TKey>
                where TKey : notnull, IEquatable<TKey>;
        }
    }
}

