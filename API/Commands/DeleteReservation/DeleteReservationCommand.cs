using MediatR;

namespace API.Commands.DeleteReservation;

public record DeleteReservationCommand(int reservationId) : IRequest<bool>
{
}
