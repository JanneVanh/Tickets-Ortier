using API.Dtos;
using API.Enums;
using Core.Interfaces;
using MediatR;

namespace API.Queries.SeatsForShow;

public class SeatsForShowQueryHandler(
    ISeatRepository seatRepository)
    : IRequestHandler<SeatsForShowQuery, List<SeatDto>>
{
    private readonly ISeatRepository _seatRepository = seatRepository;

    public async Task<List<SeatDto>> Handle(SeatsForShowQuery request, CancellationToken cancellationToken)
    {
        var allSeats = await _seatRepository.GetSeatsAsync();
        var reservedSeats = await _seatRepository.GetSeatsForShowAsync(request.ShowId);

        return allSeats.Select(s => new SeatDto
        {
            Row = s.Row,
            Number = s.Number,
            Status = reservedSeats.Select(rs => rs.Name).Contains(s.Name) ? SeatStatus.Reserved : SeatStatus.Available,
        }).ToList();
    }
}
