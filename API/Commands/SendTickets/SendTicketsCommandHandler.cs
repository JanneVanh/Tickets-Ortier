using API.Services.Contracts;
using Core.Interfaces;
using MediatR;

namespace API.Commands.SendTickets;

public class SendTicketsCommandHandler(
    IReservationRepository reservationRepository,
    IPdfService pdfService,
    IEmailService emailService) 
    : IRequestHandler<SendTicketsCommand>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IPdfService _pdfService = pdfService;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(SendTicketsCommand request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetReservationsToSendTickets();

        foreach (var reservation in reservations)
        {
            var tickets = await _pdfService.GenerateAllTicketsPdfAsync(reservation);

            var body = $@"
                <html>
                  <body>
                    <p>Beste {reservation.SurName} {reservation.Name},</p>
                    <br/>
                    <p>U reserveerde {reservation.NumberOfAdults} ticket(s) voor volwassenen en {reservation.NumberOfChildren} voor kinderen.</p>
                    <br/>
                    <p>U kan uw tickets terugvinden als bijlage.</p>
                    <br/>
                    <p>Wij kijken er alvast naar uit om voor u te zingen!</p>
                  </body>
                </html>";

            var subject = $"Tickets koor Ortier";

            await _emailService.SendEmailWithAttachments(subject, body, reservation.Email, [(tickets, "Tickets Koor Ortier")]);

            reservation.EmailSent = true;
            _reservationRepository.UpdateReservation(reservation);
            await _reservationRepository.SaveChangesAsync();
        }
    }
}
