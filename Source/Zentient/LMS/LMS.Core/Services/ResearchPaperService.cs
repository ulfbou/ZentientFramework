using LMS.Core.Entities;
using LMS.Core.Identity;
using LMS.Core.Services;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;

namespace LMS.Core.Services
{
    // Assuming ResearchPaper implements IEntity<Guid>
    public class ResearchPaperService<TContext>
        : CRUDService<ResearchPaper, Guid>, IResearchPaperService<TContext>
        where TContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ResearchPaperService(TContext dbContext) : base(dbContext) { }
    }
}
