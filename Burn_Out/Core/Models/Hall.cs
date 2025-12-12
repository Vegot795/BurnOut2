using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
        public DateTime? ReservationBegin { get; set; }
        public DateTime? ReservationEnd { get; set; }
        public int? RezervedBy { get; set; }
    }
}
