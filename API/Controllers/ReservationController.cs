﻿using API.Commands.SendReservationConfirmation;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class ReservationController(IReservationRepository reservationRepository, IMediator mediator) : ControllerBase
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult> GetReservations()
    {
        var result = await _reservationRepository.GetReservationsAsync();
        if (result == null) return NoContent();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReservation([FromBody] Reservation reservation)
    {
        var request = new CreateReservationCommand
        {
            Reservation = reservation
        };

        var result = await _mediator.Send(request);

        return Ok(result);
    }

    [HttpPut("seats/{reservationId}")]
    public async Task<ActionResult> AssignSeats([FromBody] List<Seat> seats, int reservationId)
    {
        await _reservationRepository.AssignSeatsAsync(seats, reservationId);

        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateReservation([FromBody] Reservation reservation)
    {
        _reservationRepository.UpdateReservation(reservation);
        if (await _reservationRepository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem updating reservation");
    }

    [HttpDelete("{reservationId}")]
    public async Task<ActionResult> DeleteReservation(int reservationId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        if (reservation == null) return NotFound();

        _reservationRepository.DeleteReservation(reservation);
        if (await _reservationRepository.SaveChangesAsync())
            return NoContent();

        return BadRequest("Problem deleting reservation");
    }
}
