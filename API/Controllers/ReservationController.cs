using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class ReservationController(IReservationRepository reservationRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetReservations()
    {
        var result = await reservationRepository.GetReservationsAsync();
        if (result == null) return NoContent();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReservation([FromBody] Reservation reservation)
    {
        var result = await reservationRepository.CreateReservation(reservation);

        return Ok(result);
    }

    [HttpPut("seats/{reservationId}")]
    public async Task<ActionResult> AssignSeats([FromBody] List<Seat> seats, int reservationId)
    {
        await reservationRepository.AssignSeatsAsync(seats, reservationId);

        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateReservation([FromBody] Reservation reservation)
    {
        reservationRepository.UpdateReservation(reservation);
        if (await reservationRepository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem updating reservation");
    }

    [HttpDelete("{reservationId}")]
    public async Task<ActionResult> DeleteReservation(int reservationId)
    {
        var reservation = await reservationRepository.GetReservationByIdAsync(reservationId);
        if (reservation == null) return NotFound();

        reservationRepository.DeleteReservation(reservation);
        if (await reservationRepository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem deleting reservation");
    }
}
