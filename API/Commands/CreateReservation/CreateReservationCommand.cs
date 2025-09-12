using Core.Entities;
using MediatR;

namespace API.Commands.SendReservationConfirmation;

public class CreateReservationCommand : IRequest<bool>
{
    public required Reservation Reservation { get; set; }
}
