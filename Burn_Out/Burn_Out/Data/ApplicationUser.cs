using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Burn_Out.Data
{
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

        [PersonalData]
        [StringLength(200)]
        public string? Address { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? City { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? State { get; set; }

        [PersonalData]
        [StringLength(20)]
        public string? ZipCode { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedAt { get; set; }
    }

}
