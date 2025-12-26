using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class SeatRepository(TicketContext ticketContext) : ISeatRepository
{
    public async Task DeleteSeatsForReservationAsync(int reservationId)
    {
        var seatsToSetAsAvailable = await ticketContext.ReservationSeats.Where(r => r.ReservationId == reservationId).ToListAsync();
        ticketContext.ReservationSeats.RemoveRange(seatsToSetAsAvailable);
        return;
    }

    public async Task<IReadOnlyCollection<Seat>> GetSeatsAsync()
    {
        return await ticketContext.Seats.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Seat>> GetSeatsByIdAsync(List<int> seatIds)
    {
        var seats = await GetSeatsAsync();
        return seats.Where(s => seatIds.Contains(s.Id)).ToList();
    }

    public async Task<IReadOnlyCollection<ReservationSeat>> GetSeatsForReservationsAsync()
    {
        return await ticketContext.ReservationSeats
            .Include(rs => rs.Seat)
            .ToListAsync();
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

