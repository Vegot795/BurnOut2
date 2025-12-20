using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Models;

namespace Application.Services;

public class HallReservationService
{
    private readonly ApplicationDbContext _db;
    public HallReservationService(ApplicationDbContext db) => _db = db;

    public async Task<bool> CanReserveHallAsync(int hallId, DateTime start, DateTime end)
    {
        // Ensure hall exists and is marked available
        var hall = await _db.Halls.FindAsync(hallId);
        if (hall is null) return false;
        if (!hall.IsAvailable) return false;

        // Check for overlapping reservations (any existing where start < newEnd && end > newStart)
        return !await _db.HallReservations
            .AnyAsync(r => r.HallId == hallId &&
                           r.StartTime < end &&
                           r.EndTime > start);
    }

    public async Task<bool> ReserveHallAsync(int hallId, string userId, DateTime start, DateTime end)
    {
        if (end <= start) throw new ArgumentException("End must be after start.", nameof(end));

        // Quick check: can reserve?
        if (!await CanReserveHallAsync(hallId, start, end))
            return false;

        // Use a transaction to ensure reservation and hall update are atomic
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Re-check within transaction to avoid race conditions
            var overlap = await _db.HallReservations
                .AnyAsync(r => r.HallId == hallId && r.StartTime < end && r.EndTime > start);
            if (overlap)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var hall = await _db.Halls.FindAsync(hallId);
            if (hall is null || !hall.IsAvailable)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var reservation = new HallReservation
            {
                HallId = hallId,
                UserId = userId,
                StartTime = start,
                EndTime = end
            };

            await _db.HallReservations.AddAsync(reservation);

            // update hall state to reflect reservation
            hall.ReservationBegin = start;
            hall.ReservationEnd = end;
            hall.IsAvailable = false;

            _db.Halls.Update(hall);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}