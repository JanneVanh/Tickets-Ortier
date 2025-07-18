namespace Core.Entities;

public class ReservationSeat : BaseEntity
{
    public required int ReservationId { get; set; }
    public required Reservation Reservation { get; set; }
    public required int SeatId { get; set; }
    public required Seat Seat { get; set; }
    public required int ShowId { get; set; } // <-- Make this a mapped property
}

