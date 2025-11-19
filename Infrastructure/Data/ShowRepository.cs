using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ShowRepository(TicketContext ticketContext) : IShowRepository
{
    private readonly TicketContext _ticketContext = ticketContext;

    public async Task<Show?> GetShowByIdAsync(int id)
    {
        return await _ticketContext.Shows.FindAsync(id);
    }

    public async Task<IReadOnlyCollection<Show>> GetShowsAsync()
    {
        return await _ticketContext.Shows.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Show>> GetShowsWithTicketsAsync()
    {
        return await _ticketContext.Shows.Where(s => s.AvailableTickets > 0).ToListAsync();
    }

    public void UpdateShow(Show show)
    {
        _ticketContext.Entry(show).State = EntityState.Modified;
    }
}