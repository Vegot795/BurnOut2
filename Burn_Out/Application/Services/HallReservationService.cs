using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Application.Services;

public class HallReservationService
{
    private readonly ApplicationDbContext _db;
    public HallReservationService(ApplicationDbContext db) => _db = db;

    public async Task<bool> CanReserveHallAsync(int hallId, DateTime start, DateTime end)
    {
        return !await _db.HallReservations
            .AnyAsync(r => r.HallId == hallId &&
                           r.StartTime < end &&
                           r.EndTime > start);
    }

    public async Task<bool> ReserveHallAsync(int hallId, string userId, DateTime start, DateTime end)
    {
        if (!await CanReserveHallAsync(hallId, start, end))
            return false;

        var reservation = new HallReservation
        {
            HallId = hallId,
            UserId = userId,
            StartTime = start,
            EndTime = end
        };
        _db.HallReservations.Add(reservation);
        await _db.SaveChangesAsync();
        return true;
    }
}