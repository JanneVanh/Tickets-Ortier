using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ShowRepository(TicketContext ticketContext) : IShowRepository
{
    private readonly TicketContext _ticketContext = ticketContext;

    public async Task<IReadOnlyCollection<Show>> GetShowsAsync()
    {
        return await _ticketContext.Shows.ToListAsync();
    }
}

