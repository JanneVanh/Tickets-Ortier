using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class SeatRepository(TicketContext ticketContext) : ISeatRepository
{
    public async Task<IReadOnlyCollection<Seat>> GetSeatsAsync()
    {
        return await ticketContext.Seats.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Seat>> GetSeatsForReservationAsync(int reservationId)
    {
        return await ticketContext.ReservationSeats
            .Where(rs => rs.ReservationId == reservationId)
            .Select(rs => rs.Seat)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Seat>> GetSeatsForShowAsync(int showId)
    {
        return await ticketContext.ReservationSeats
            .Where(rs => rs.ShowId == showId)
            .Select(rs => rs.Seat)
            .ToListAsync();
    }
}

