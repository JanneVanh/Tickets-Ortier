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
}
