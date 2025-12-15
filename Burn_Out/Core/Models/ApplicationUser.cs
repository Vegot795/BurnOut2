using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [PersonalData]
    [StringLength(100)]
    public string? LastName { get; set; }

    [PersonalData]
    public DateTime? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastModifiedAt { get; set; }
}
