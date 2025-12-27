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
        var hall = await _db.Halls.FindAsync(hallId);
        if (hall is null) return false;
        if (!hall.IsAvailable) return false;

        return !await _db.HallReservations
            .AnyAsync(r => r.HallId == hallId &&
                           r.StartTime < end &&
                           r.EndTime > start);
    }

    public async Task<bool> ReserveHallAsync(int hallId, string userId, DateTime start, DateTime end)
    {
        if (end <= start) throw new ArgumentException("End must be after start.", nameof(end));

        if (!await CanReserveHallAsync(hallId, start, end))
            return false;

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
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

    public async Task CancelReservationAsync(int reservationId)
    {
        var reservation = await _db.HallReservations.FindAsync(reservationId);
        if (reservation is null) throw new ArgumentException("Reservation not found.", nameof(reservationId));
        var hall = await _db.Halls.FindAsync(reservation.HallId);
        if (hall is null) throw new InvalidOperationException("Associated hall not found.");
        _db.HallReservations.Remove(reservation);

        hall.IsAvailable = true;
        hall.ReservationBegin = null;
        hall.ReservationEnd = null;
        _db.Halls.Update(hall);
        await _db.SaveChangesAsync();
    }
}