using API.Commands.CreateReservation;
using API.Commands.DeleteReservation;
using API.Commands.ExportReservations;
using API.Commands.SendReservationConfirmation;
using API.Commands.SendTickets;
using API.Queries.ReservationOverview;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ReservationController(IReservationRepository reservationRepository, IMediator mediator) : ControllerBase
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetReservations()
    {
        var query = new ReservationOverviewQuery();
        var result = await _mediator.Send(query);

        if (result == null) return NoContent();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReservation([FromBody] CreateReservationRequest body)
    {
        var command = new CreateReservationCommand
        {
            Reservation = body.Reservation,
            SeatIds = body.SeatIds,
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateReservation([FromBody] Reservation reservation)
    {
        _reservationRepository.UpdateReservation(reservation);
        if (await _reservationRepository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem updating reservation");
    }

    [HttpDelete("{reservationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteReservation(int reservationId)
    {
        var command = new DeleteReservationCommand(reservationId);

        var result = await _mediator.Send(command);

        if (result)
            return NoContent();

        return BadRequest("Problem deleting reservation");
    }

    [HttpPost("sendTickets")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendTickets()
    {
        var command = new SendTicketsCommand();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("export")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ExportReservations()
    {
        var query = new ReservationOverviewQuery();
        var reservationDtos = await _mediator.Send(query);

        var command = new ExportReservationCommand
        {
            ReservationDtos = reservationDtos
        };
        var result = await _mediator.Send(command);

        if (result is null)
            return NoContent();

        return File(
            result,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"reservaties-{DateTime.Now:yyyyMMdd-HHmm}.xlsx"
        );
    }
}
