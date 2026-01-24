using Core.Models;
using Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHallRepository
    {
        Task<List<HallModel>> GetAllAsync();
        Task<HallModel?> GetByIdAsync(int id);
        Task AddAsync(HallModel hall);
        Task UpdateAsync(HallModel hall);
        Task DeleteAsync(int id);
    }
}
