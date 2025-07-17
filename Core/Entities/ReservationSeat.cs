namespace Core.Entities;

public class ReservationSeat
{
    public required Reservation Reservation { get; set; }
    public required Seat Seat { get; set; }
}

