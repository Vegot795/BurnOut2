using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;


namespace Infrastructure.Repositories
{
    public class TerminsReposiory : ITerminsRepository
    {
        private readonly ApplicationDbContext _context;

        public TerminsReposiory(ApplicationDbContext contex)
        {
            _context = contex;
        }

        public async Task AddTerminAsync(TerminsModel termin)
        {
            await _context.Termins.AddAsync(termin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTerminAsync(int id)
        {
            var termin = await _context.Termins.FindAsync(id);
            if (termin != null)
            {
                _context.Termins.Remove(termin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TerminsModel>> GetAllTerminsAsync()
        {
            return await _context.Termins.ToListAsync();
        }

        public async Task<TerminsModel?> GetTerminByIdAsync(int id)
        {
            return await _context.Termins.FindAsync(id);
        }

        public async Task UpdateTerminAsync(TerminsModel termin)
        {
            _context.Termins.Update(termin);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TerminsModel>> AddTerminsWitDate(DateOnly selectedDate)
        {
            List<TerminsModel> termins = await _context.Termins
                        .Where(t => t.Date == selectedDate)
                        .OrderBy(t => t.StartTime)
                        .ToListAsync();
            return termins;
        }
    }
}
