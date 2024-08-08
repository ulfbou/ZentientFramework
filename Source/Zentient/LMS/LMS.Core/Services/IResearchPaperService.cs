using LMS.Core.Entities;
using LMS.Core.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LMS.Core.Services
{
    public interface IResearchPaperService<TContext>
        : ICRUDService<ResearchPaper, TContext>
        where TContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    { }
}
