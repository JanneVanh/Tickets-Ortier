using API.Enums;

namespace Core.Interfaces;

public interface ISeatHoldRepository
{
    Task<List<int>> GetHoldedSeatsForShowAsync(int showId);
    Task<SeatStatus> HoldSeatAsync(int seatId, int showId);
    Task<SeatStatus> UnHoldSeatAsync(int seatId, int showId);
    Task<int> CleanupExpiredHoldsAsync();
}
