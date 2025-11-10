using API.Dtos;
using API.Enums;
using Core.Interfaces;
using MediatR;

namespace API.Queries.SeatsForShow;

public class SeatsForShowQueryHandler(
    ISeatRepository seatRepository,
    ISeatHoldRepository seatHoldRepository)
    : IRequestHandler<SeatsForShowQuery, List<SeatDto>>
{
    private readonly ISeatRepository _seatRepository = seatRepository;
    private readonly ISeatHoldRepository _seatHoldRepository = seatHoldRepository;

    public async Task<List<SeatDto>> Handle(SeatsForShowQuery request, CancellationToken cancellationToken)
    {
        var allSeats = await _seatRepository.GetSeatsAsync();
        var reservedSeats = await _seatRepository.GetSeatsForShowAsync(request.ShowId);

        var holdedSeatIds = await _seatHoldRepository.GetHoldedSeatsForShowAsync(request.ShowId);
        var holdedSeats = await _seatRepository.GetSeatsByIdAsync(holdedSeatIds);

        return allSeats.Select(s => new SeatDto
        {
            Id = s.Id,
            Row = s.Row,
            Number = s.Number,
            Status = reservedSeats.Select(rs => rs.Name).Contains(s.Name) ? SeatStatus.Reserved : 
                (holdedSeats.Select(hs => hs.Name).Contains(s.Name) ? SeatStatus.Reserved : SeatStatus.Available),
        }).ToList();
    }
}
