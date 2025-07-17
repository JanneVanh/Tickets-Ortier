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
}
