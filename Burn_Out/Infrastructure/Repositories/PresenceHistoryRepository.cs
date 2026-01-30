using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PresenceHistoryRepository : IPresenceHistory
    {
        private readonly ApplicationDbContext _contex;

        public PresenceHistoryRepository(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task AddPresenceAsync(string UserId)
        {
            DateTime now = DateTime.Now;
            PresenceHistoryModel presenceHistory = new PresenceHistoryModel
            {
                PresenceDate = now
            };
            await _contex.PresenceHistories.AddAsync(presenceHistory);
            await _contex.SaveChangesAsync();
        }
    }
}