using LMS.Core.Identity;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task EnsureRolesAsync(RoleManager<Role> roleManager)
    {
        var teacherExists = await roleManager.RoleExistsAsync(Role.Teacher);
        if (!teacherExists)
        {
            await roleManager.CreateAsync(new Role(Role.Teacher));
        }

        var studentExists = await roleManager.RoleExistsAsync(Role.Student);
        if (!studentExists)
        {
            await roleManager.CreateAsync(new Role(Role.Student));
        }
    }
}
