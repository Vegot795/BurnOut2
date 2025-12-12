using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class AppIdentityUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
