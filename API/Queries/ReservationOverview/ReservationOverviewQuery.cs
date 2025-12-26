using API.Dtos;
using MediatR;

namespace API.Queries.ReservationOverview;

public class ReservationOverviewQuery : IRequest<List<ReservationDto>>
{
}
