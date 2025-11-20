using Core.Entities;

namespace Core.Interfaces;

public interface ISeatRepository
{
    Task<IReadOnlyCollection<Seat>> GetSeatsAsync();
    Task<IReadOnlyCollection<Seat>> GetSeatsForShowAsync(int showId);
    Task<IReadOnlyCollection<Seat>> GetSeatsForReservationAsync(int reservationId);
    Task<IReadOnlyCollection<Seat>> GetSeatsByIdAsync(List<int> seatIds);
    Task DeleteSeatsForReservationAsync(int reservationId);
}
