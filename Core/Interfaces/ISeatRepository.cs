using Core.Entities;

namespace Core.Interfaces;

public interface ISeatRepository
{
    Task<IReadOnlyCollection<Seat>> GetSeatsAsync();
}
