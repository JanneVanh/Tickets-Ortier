using API.Dtos;
using MediatR;

namespace API.Queries.SeatsForShow;

public class SeatsForShowQuery : IRequest<List<SeatDto>>
{
    public required int ShowId { get; set; }
}
