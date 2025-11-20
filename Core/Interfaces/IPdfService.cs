using Core.Entities;

namespace API.Services.Contracts;

public interface IPdfService
{
    Task<byte[]> GenerateTicketPdfAsync(Reservation reservation, Seat seat);
    Task<byte[]> GenerateAllTicketsPdfAsync(Reservation reservation);
}
