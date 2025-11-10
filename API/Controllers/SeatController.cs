using API.Dtos;
using API.Enums;
using API.Queries.SeatsForShow;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class SeatController(ISeatRepository seatRepository, IMediator mediator, ISeatHoldRepository seatHoldRepository) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<Seat>>> GetAllSeatsAsync()
    {
        var result = await seatRepository.GetSeatsAsync();
        if (result is null) return NoContent();

        return Ok(result);
    }

    [HttpGet("show/{showId}")]
    public async Task<ActionResult<List<Seat>>> GetSeatsForShowAsync(int showId)
    {
        var request = new SeatsForShowQuery()
        {
            ShowId = showId
        };
        var result = await _mediator.Send(request);

        if (result is null) return NoContent();
        return Ok(result);
    }

    [HttpGet("reservation/{reservationId}")]
    public async Task<ActionResult<List<Seat>>> GetSeatsForReservationAsync(int reservationId)
    {
        var result = await seatRepository.GetSeatsForReservationAsync(reservationId);
        if (result is null) return NoContent();

        return Ok(result);
    }

    [HttpPost("Hold")]
    public async Task<ActionResult<SeatStatus>> HoldSeat([FromBody] ShowSeatDto showSeat)
    {
        var result = await seatHoldRepository.HoldSeatAsync(showSeat.SeatId, showSeat.ShowId);
        return Ok(result);
    }


    [HttpPost("Unhold")]
    public async Task<ActionResult<SeatStatus>> UnHoldSeat([FromBody] ShowSeatDto showSeat)
    {
        var result = await seatHoldRepository.UnHoldSeatAsync(showSeat.SeatId, showSeat.ShowId);
        return Ok(result);
    }

    [HttpPost("cleanup-expired-holds")]
    public async Task<ActionResult<object>> CleanupExpiredHolds()
    {
        var cleanedUpCount = await seatHoldRepository.CleanupExpiredHoldsAsync();
        return Ok(new { message = $"Cleaned up {cleanedUpCount} expired seat holds", count = cleanedUpCount });
    }
}
