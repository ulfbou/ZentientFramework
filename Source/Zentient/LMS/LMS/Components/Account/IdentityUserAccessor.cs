
using LMS.Core.Identity;
using Microsoft.AspNetCore.Identity;

namespace LMS.Components.Account
{
    public sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IdentityRedirectManager redirectManager = redirectManager;

        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
