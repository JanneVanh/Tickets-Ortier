using API.Enums;

namespace Core.Entities;

public class SeatHold
{
    public int Id { get; set; } // Primary key
    public required int SeatId { get; set; }
    public required int ShowId { get; set; }
    public DateTime HoldStartTime { get; set; }
    public DateTime HoldExpiryTime { get; set; }
    public SeatStatus Status { get; set; } = SeatStatus.Selected;
}

