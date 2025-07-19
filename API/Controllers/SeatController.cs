using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class SeatController(ISeatRepository seatRepository) : ControllerBase
{
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
        var result = await seatRepository.GetSeatsForShowAsync(showId);
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
}
