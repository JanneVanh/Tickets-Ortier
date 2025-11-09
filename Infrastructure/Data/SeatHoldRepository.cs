using API.Enums;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class SeatHoldRepository(TicketContext ticketContext) : ISeatHoldRepository
{
    public async Task<SeatStatus> HoldSeatAsync(int seatId, int showId)
    {
        var seatNotAvailable = ticketContext.SeatHolds.Any(s => s.ShowId == showId && s.SeatId == seatId);
        if (seatNotAvailable)
            return SeatStatus.Reserved;

        var seatHold = new SeatHold
        {
            SeatId = seatId,
            ShowId = showId,
            Status = SeatStatus.Selected,
            HoldStartTime = DateTime.UtcNow,
            HoldExpiryTime = DateTime.UtcNow.AddMinutes(5),
        };

        await ticketContext.SeatHolds.AddAsync(seatHold);
        await ticketContext.SaveChangesAsync();

        return SeatStatus.Selected;
    }

    public async Task<SeatStatus> UnHoldSeatAsync(int seatId, int showId)
    {
        var seatHold = ticketContext.SeatHolds.FirstOrDefault(s => s.ShowId == showId && s.SeatId == seatId);
        if (seatHold is null)
            return SeatStatus.Available;

        ticketContext.SeatHolds.Remove(seatHold);
        await ticketContext.SaveChangesAsync();

        return SeatStatus.Available;
    }
}

