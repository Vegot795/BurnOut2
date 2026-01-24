using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Measurement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int? MaxDeadLift { get; set; }
        public int? MaxBenchPress { get; set; }
        public int? MaxSquat { get; set; }
        public int? BackWide { get; set; }
        public int? BicepsCircumference { get; set; }
        public int? ThighCircumference { get; set; }
        public int? ChestCircumference { get; set; }
        public int? BellyCircumference { get; set; }
    }
}
