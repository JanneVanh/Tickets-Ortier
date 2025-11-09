using Core.Entities;

namespace API.Commands.CreateReservation;

public class CreateReservationRequest
{
    public required Reservation Reservation { get; set; }
    public required List<int> SeatIds { get; set; }
}
