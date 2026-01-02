using API.Dtos;
using MediatR;

namespace API.Commands.ExportReservations;

public class ExportReservationCommand() : IRequest<Byte[]>
{
    public List<ReservationDto> ReservationDtos { get; set; } = [];
}