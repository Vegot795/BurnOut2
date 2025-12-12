// Infrastructure/Data/IdentitySeeder.cs
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager,
                                                        UserManager<ApplicationUser> userManager)
        {   
            // 1. Seed roles    
            string[] roles = new[] { "Client", "Employee", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // 2. Seed default admin
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Pass!23";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }

                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign 'Admin' role to admin user: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
