using Core.Entities;

namespace Core.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task<Reservation?> CreateReservation(Reservation reservation);
    void UpdateReservation(Reservation reservation);
    void DeleteReservation(Reservation reservation);
    Task AssignSeatsAsync(IEnumerable<int> seatIds, Reservation reservation);
    Task<bool> SaveChangesAsync();
}
