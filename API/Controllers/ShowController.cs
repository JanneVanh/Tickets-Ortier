using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ShowController(IShowRepository showRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Show>>> GetAllShowsAsync()
    {
        var result = await showRepository.GetShowsAsync();
        if (result is null) return NotFound();

        return Ok(result);
    }
}
