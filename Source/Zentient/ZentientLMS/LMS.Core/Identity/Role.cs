using Microsoft.AspNetCore.Identity;

namespace LMS.Core.Identity
{
    public class Role : IdentityRole<int>
    {
        public static string Teacher => "Teacher";
        public static string Student => "Student";
        public static string Admin => "Admin";

        public Role() : base()
        {
        }

        public Role(string roleName) : base(roleName)
        {
        }
    }
}