namespace Core.Interfaces
{
    public interface IPresenceHistory
    {
        Task AddPresenceAsync(string userId);
    }
}