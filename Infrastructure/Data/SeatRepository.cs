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
}

