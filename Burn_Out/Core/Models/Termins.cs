using System;

namespace Core.Models
{
    public class Termins
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; } // Only hour and minute
        public TimeOnly EndTime { get; set; }   // Only hour and minute
        public bool IsReserved { get; set; }
        public string? ReservedByUserId { get; set; }

        public Termins() { }

        public Termins(int id, string title, TimeOnly startTime, TimeOnly endTime)
        {
            Id = id;
            Title = title;
            StartTime = startTime;
            EndTime = endTime;
            IsReserved = false;
            ReservedByUserId = null;
        }

        public void Reserve(string userId)
        {
            if (IsReserved)
                throw new InvalidOperationException("Termin is already reserved.");

            IsReserved = true;
            ReservedByUserId = userId;
        }

        public void CancelReservation()
        {
            IsReserved = false;
            ReservedByUserId = null;
        }
    }
}
