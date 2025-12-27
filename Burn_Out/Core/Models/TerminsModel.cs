using System;

namespace Core.Models
{
    public class TerminsModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly? Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsReserved { get; set; }
        public string? ReservedByUserId { get; set; }

        public TerminsModel() { }

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
