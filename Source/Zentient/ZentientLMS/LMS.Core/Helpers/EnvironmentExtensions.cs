using Microsoft.AspNetCore.Hosting;

namespace LMS.Core.Helpers
{
    public static partial class Environment
    {

        public static bool IsDevelopment()
        {
            return IsDevelopmentEnvironment("Development");
        }

        public static bool IsStaging()
        {
            return IsDevelopmentEnvironment("Development");
        }

        public static bool IsProduction()
        {
            return IsDevelopmentEnvironment("Production");
        }

        private static bool IsDevelopmentEnvironment(string environmentName)
        {
            return GetDevelopmentEnvironment() == environmentName;
        }

        public static string? GetDevelopmentEnvironment()
        {
            return System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

    }
}
