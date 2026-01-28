using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HallRepository : IHallRepository
    {
        private readonly ApplicationDbContext _contex;

        public HallRepository(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<List<HallModel>> GetAllAsync()
        {
            return await _contex.Halls.ToListAsync();
        }

        public async Task<HallModel?> GetByIdAsync(int id)
        {
            return await _contex.Halls.FindAsync(id);
        }

        public async Task AddAsync(HallModel hall)
        {
            await _contex.Halls.AddAsync(hall);
            await _contex.SaveChangesAsync();
        }

        public async Task UpdateAsync(HallModel hall)
        {
            _contex.Halls.Update(hall);
            await _contex.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hall = await _contex.Halls.FindAsync(id);
            if (hall != null)
            {
                _contex.Halls.Remove(hall);
                await _contex.SaveChangesAsync();
            }
        }
    }
}