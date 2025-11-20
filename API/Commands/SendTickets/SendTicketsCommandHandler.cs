using API.Services.Contracts;
using Core.Interfaces;
using MediatR;

namespace API.Commands.SendTickets;

public class SendTicketsCommandHandler(
    IReservationRepository reservationRepository,
    IPdfService pdfService) 
    : IRequestHandler<SendTicketsCommand>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IPdfService _pdfService = pdfService;

    public async Task Handle(SendTicketsCommand request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetReservationsToSendTickets();

        foreach (var reservation in reservations)
        {
            await _pdfService.GenerateAllTicketPdfsAsync(reservation);
            reservation.EmailSent = true;
            _reservationRepository.UpdateReservation(reservation);
        }
    }
}
