using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class HallModel
    {
        public int Id { get; set; }
        public string HallName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ReservationBegin { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ReservationEnd { get; set; }

        public string? ReservedBy { get; set; } // make nullable to match DB nulls
    }
}