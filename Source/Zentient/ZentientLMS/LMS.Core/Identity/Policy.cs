using Microsoft.AspNetCore.Authorization;

namespace LMS.Core.Identity
{
    public class Policy : AuthorizationPolicy
    {
        public static string RequireAnyRole = "RequireAnyRole";
        public static string RequireAdminRole = "RequireAdminRole";
        public static string RequireTeacherRole = "RequireTeacherRole";
        public static string RequireStudentRole = "RequireStudentRole";
        public static string RequireAdminOrTeacherRole = "RequireAdminOrTeacherRole";
        public static string RequireStudentOrTeacherRole = "RequireStudentOrTeacherRole";

        public Policy(
                IEnumerable<IAuthorizationRequirement> requirements,
                IEnumerable<string> authenticationSchemes)
            : base(requirements, authenticationSchemes)
        {
        }
    }
}
