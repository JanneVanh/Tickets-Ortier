using API.Dtos;
using Core.Interfaces;
using MediatR;

namespace API.Queries.ReservationOverview;

public class ReservationOverviewQueryHandler
    (IReservationRepository reservationRepository,
    ISeatRepository seatRepository)
    : IRequestHandler<ReservationOverviewQuery, List<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;

    public async Task<List<ReservationDto>> Handle(ReservationOverviewQuery request, CancellationToken cancellationToken)
    {
        var reservationEntities = await _reservationRepository.GetReservationsAsync();
        var reservationSeats = await _seatRepository.GetSeatsForReservationsAsync();

        var reservations = reservationEntities.Select(re => new ReservationDto
        {
            Id = re.Id,
            SurName = re.Name,
            Name = re.Name,
            Email = re.Email,
            ReservationDate = DateTime.Now,
            NumberOfAdults = re.NumberOfAdults,
            NumberOfChildren = re.NumberOfChildren,
            TotalPrice = re.TotalPrice,
            Remark = re.Remark,
            IsPaid = re.IsPaid,
            PaymentCode = re.PaymentCode,
            PaymentRemark = re.PaymentRemark,
            ShowId = re.ShowId,
            EmailSent = re.EmailSent,
            Seats = reservationSeats.Where(rs => rs.ReservationId == re.Id).Select(rs => rs.Seat).Select(s => s.Name).ToList(),
        });

        return reservations.ToList();
    }
}
