using Microsoft.AspNetCore.Identity;
using Zentient.Repository;

namespace LMS.Core.Entities
{
    public abstract class BaseIdentityUser : BaseIdentityUser<string>, IEntity<string>
    { }

    public abstract class BaseIdentityUser<TKey>
        : IdentityUser<TKey>, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual string Searchable { get => $"{UserName} {Email}"; }
    }
}