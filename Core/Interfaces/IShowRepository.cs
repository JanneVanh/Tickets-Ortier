using Core.Entities;

namespace Core.Interfaces;

public interface IShowRepository
{
    Task<IReadOnlyCollection<Show>> GetShowsAsync();
    Task<Show?> GetShowByIdAsync(int id);
}
