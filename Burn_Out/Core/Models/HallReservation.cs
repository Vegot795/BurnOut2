using Infrastructure.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class HallReservation
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public HallModel Hall { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

        public DateTime WhenReserved { get; set; }
    }
}