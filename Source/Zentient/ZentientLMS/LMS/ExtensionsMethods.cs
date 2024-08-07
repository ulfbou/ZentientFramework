
namespace LMS
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddIdentityCookies(this IServiceCollection services)
        {
            // Configure the application cookie options without duplicating the "Identity.Application" scheme.
            services.ConfigureApplicationCookie(options =>
            {
                // Customize the application cookie settings here
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            // If adding custom authentication schemes, ensure they have unique names
            services.AddAuthentication().AddCookie("CustomScheme", options =>
            {
                // Configure your custom scheme options here
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            // Continue with any other custom configurations

            return services;
        }

    }
}