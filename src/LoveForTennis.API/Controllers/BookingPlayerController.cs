using Microsoft.AspNetCore.Mvc;
using LoveForTennis.Application.DTOs;
using LoveForTennis.Application.Interfaces;

namespace LoveForTennis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingPlayerController : ControllerBase
{
    private readonly IBookingPlayerService _bookingPlayerService;

    public BookingPlayerController(IBookingPlayerService bookingPlayerService)
    {
        _bookingPlayerService = bookingPlayerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingPlayerDto>>> GetAll()
    {
        try
        {
            var bookingPlayers = await _bookingPlayerService.GetAllBookingPlayersAsync();
            return Ok(bookingPlayers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingPlayerDto>> GetById(int id)
    {
        try
        {
            var bookingPlayer = await _bookingPlayerService.GetBookingPlayerByIdAsync(id);
            if (bookingPlayer == null)
                return NotFound();

            return Ok(bookingPlayer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<IEnumerable<BookingPlayerDto>>> GetByBookingId(int bookingId)
    {
        try
        {
            var bookingPlayers = await _bookingPlayerService.GetBookingPlayersByBookingIdAsync(bookingId);
            return Ok(bookingPlayers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("player/{playerUserId}")]
    public async Task<ActionResult<IEnumerable<BookingPlayerDto>>> GetByPlayerUserId(string playerUserId)
    {
        try
        {
            var bookingPlayers = await _bookingPlayerService.GetBookingPlayersByPlayerUserIdAsync(playerUserId);
            return Ok(bookingPlayers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingPlayerDto>> Create(BookingPlayerDto bookingPlayerDto)
    {
        try
        {
            var created = await _bookingPlayerService.CreateBookingPlayerAsync(bookingPlayerDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookingPlayerDto>> Update(int id, BookingPlayerDto bookingPlayerDto)
    {
        try
        {
            if (id != bookingPlayerDto.Id)
                return BadRequest();

            var updated = await _bookingPlayerService.UpdateBookingPlayerAsync(bookingPlayerDto);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _bookingPlayerService.DeleteBookingPlayerAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}