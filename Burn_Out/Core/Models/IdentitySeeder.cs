
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Core.Models;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Enum.GetValues<UserRoles>())
        {
            var roleName = role.ToString();

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
