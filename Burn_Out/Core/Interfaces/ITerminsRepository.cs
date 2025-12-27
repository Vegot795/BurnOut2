using Core.Models;

namespace Core.Interfaces
{
    public interface ITerminsRepository
    {
        Task<List<TerminsModel>> GetAllTerminsAsync();
        Task<TerminsModel?> GetTerminByIdAsync(int id);
        Task AddTerminAsync(TerminsModel termin);
        Task UpdateTerminAsync(TerminsModel termin);
        Task DeleteTerminAsync(int id);
    }
}
