using API.Enums;

namespace Core.Interfaces;

public interface ISeatHoldRepository
{
    Task<SeatStatus> HoldSeatAsync(int seatId, int showId);
    Task<SeatStatus> UnHoldSeatAsync(int seatId, int showId);
}
