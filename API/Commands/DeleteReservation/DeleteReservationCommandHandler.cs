using Core.Interfaces;
using MediatR;

namespace API.Commands.DeleteReservation;

public class DeleteReservationCommandHandler(IReservationRepository reservationRepository, ISeatRepository seatRepository, IShowRepository showRepository) : IRequestHandler<DeleteReservationCommand, bool>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;
    private readonly IShowRepository _showRepository = showRepository;

    public async Task<bool> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(request.reservationId);
        if (reservation == null) return false;

        await _seatRepository.DeleteSeatsForReservationAsync(request.reservationId);

        var show = await _showRepository.GetShowByIdAsync(reservation.ShowId);
        if (show is null) return false;
        show.AvailableTickets += reservation.NumberOfAdults + reservation.NumberOfChildren;
        _showRepository.UpdateShow(show);

        _reservationRepository.DeleteReservation(reservation);
        await _reservationRepository.SaveChangesAsync();
        return true;
    }
}
