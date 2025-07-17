using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ReservationRepository(TicketContext ticketContext) : IReservationRepository
{
    private readonly TicketContext _ticketContext = ticketContext;

    public void CreateReservation(Reservation reservation)
    {
        _ticketContext.Reservations.Add(reservation);
    }

    public void DeleteReservation(Reservation reservation)
    {
        _ticketContext.Reservations.Remove(reservation);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsAsync()
    {
        return await _ticketContext.Reservations.ToListAsync();
    }

    public void UpdateReservation(Reservation reservation)
    {
        _ticketContext.Entry(reservation).State = EntityState.Modified;
    }
}

