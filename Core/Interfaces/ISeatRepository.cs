using Core.Entities;

namespace Core.Interfaces;

public interface ISeatRepository
{
    Task<IReadOnlyCollection<Seat>> GetSeatsAsync();
    Task<IReadOnlyCollection<Seat>> GetSeatsForShowAsync(int showId);
    Task<IReadOnlyCollection<Seat>> GetSeatsForReservationAsync(int reservationId);
