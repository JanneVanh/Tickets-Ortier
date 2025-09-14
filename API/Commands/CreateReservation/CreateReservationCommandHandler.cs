using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace API.Commands.SendReservationConfirmation;

public class CreateReservationCommandHandler(IEmailService emailService, IShowRepository showRepository, IReservationRepository reservationRepository)
    : IRequestHandler<CreateReservationCommand, Reservation>
{
    private readonly IEmailService _emailService = emailService;
    private readonly IShowRepository _showRepository = showRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;

    public async Task<Reservation> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.CreateReservation(request.Reservation);
        if (reservation == null) throw new InvalidOperationException("Couldn't save reservation");

        var show = await _showRepository.GetShowByIdAsync(reservation.ShowId);

        if (show is null) throw new InvalidOperationException("Show not found");

        var body = $@"
    <html>
      <body>
        <p>Beste {reservation.SurName} {reservation.Name},</p>
        <br/>
        <p>U reserveerde {reservation.NumberOfAdults} ticket(s) voor volwassenen en {reservation.NumberOfChildren} voor kinderen.</p>
        <p>Gelieve &euro; {reservation.TotalPrice} over te schrijven op rekeningnummer BE. Vermeld de code <b>{reservation.PaymentCode}</b> als mededeling.</p>
        <br/>
        <p>Enkele dagen voor de show zal u uw tickets via email ontvangen.</p>
        <br/>
        <p>Wij kijken er alvast naar uit om voor u te zingen!</p>
      </body>
    </html>";

        var subject = $"Reservatie koor Ortier op {show.Day} om {show.CurtainsUp}";

        await _emailService.SendEmail(subject, body, reservation.Email);

        return reservation;
    }
}
