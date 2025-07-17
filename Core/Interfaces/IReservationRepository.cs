using Core.Entities;

namespace Core.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsAsync();
    void CreateReservation(Reservation reservation);
    void UpdateReservation(Reservation reservation);
    void DeleteReservation(Reservation reservation);
}
