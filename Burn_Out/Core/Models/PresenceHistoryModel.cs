using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class PresenceHistoryModel : IdentityUser
    {
        public DateTime PresenceDate { get; set; }
    }
}