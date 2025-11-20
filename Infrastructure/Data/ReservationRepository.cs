using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ReservationRepository(TicketContext ticketContext) : IReservationRepository
{
    private readonly TicketContext _ticketContext = ticketContext;

    public async Task AssignSeatsAsync(IEnumerable<int> seatIds, Reservation reservation)
    {
        var existingSeats = await _ticketContext.ReservationSeats.Where(rs => rs.ReservationId == reservation.Id).ToListAsync();
        if (existingSeats is not null && existingSeats.Count > 0)
            _ticketContext.ReservationSeats.RemoveRange(existingSeats);

        foreach (var seatId in seatIds)
        {
            var seat = await _ticketContext.Seats.FirstOrDefaultAsync(s => s.Id == seatId);

            var reservationSeat = new ReservationSeat()
            {
                Seat = seat!,
                SeatId = seatId,
                ReservationId = reservation.Id,
                Reservation = reservation,
                ShowId = reservation.ShowId
            };
            _ticketContext.ReservationSeats.Add(reservationSeat);
        }
        await _ticketContext.SaveChangesAsync();
    }

    public async Task<Reservation?> CreateReservation(Reservation reservation)
    {

        _ticketContext.Reservations.Add(reservation);
        await _ticketContext.SaveChangesAsync();

        return reservation;
    }

    public void DeleteReservation(Reservation reservation)
    {
        _ticketContext.Reservations.Remove(reservation);
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await _ticketContext.Reservations.FindAsync(reservationId);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsAsync()
    {
        return await _ticketContext.Reservations.ToListAsync();
    }

    public void UpdateReservation(Reservation reservation)
    {
        _ticketContext.Entry(reservation).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _ticketContext.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsToSendTickets()
    {
        return await _ticketContext.Reservations
            .Where(r => r.IsPaid)
            .Where(r => !r.EmailSent)
            .ToListAsync();
    }
}

