using System.Security.Claims;
using HelpDesk.API.Data;
using HelpDesk.API.DTOs.Tickets;
using HelpDesk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public TicketsController(ApplicationDbContext context)
  {
    _context = context;
  }

  [HttpPost]
  public async Task<ActionResult<TicketDto>> CreateTicket(CreateTicketDto dto)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var ticket = new Ticket
    {
      Title = dto.Title,
      Description = dto.Description,
      Priority = dto.Priority,
      Status = "Open",
      CreatedAt = DateTime.UtcNow,
      UserId = userId
    };

    _context.Tickets.Add(ticket);
    await _context.SaveChangesAsync();

    var ticketDto = new TicketDto
    {
      Id = ticket.Id,
      Title = ticket.Title,
      Description = ticket.Description,
      Status = ticket.Status,
      Priority = ticket.Priority,
      CreatedAt = ticket.CreatedAt,
      UpdatedAt = ticket.UpdatedAt,
      ClosedAt = ticket.ClosedAt,
      UserId = ticket.UserId
    };

    return CreatedAtAction(
        nameof(CreateTicket),
        new { id = ticket.Id },
        ticketDto
    );
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var tickets = await _context.Tickets
        .Where(t => t.UserId == userId)
        .OrderByDescending(t => t.CreatedAt)
        .Select(t => new TicketDto
        {
          Id = t.Id,
          Title = t.Title,
          Description = t.Description,
          Status = t.Status,
          Priority = t.Priority,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt,
          ClosedAt = t.ClosedAt,
          UserId = t.UserId
        })
        .ToListAsync();

    return Ok(tickets);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TicketDto>> UpdateTicket(
      int id,
      UpdateTicketDto dto)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (ticket == null)
    {
      return NotFound();
    }

    ticket.Title = dto.Title;
    ticket.Description = dto.Description;
    ticket.Priority = dto.Priority;
    ticket.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    var ticketDto = new TicketDto
    {
      Id = ticket.Id,
      Title = ticket.Title,
      Description = ticket.Description,
      Status = ticket.Status,
      Priority = ticket.Priority,
      CreatedAt = ticket.CreatedAt,
      UpdatedAt = ticket.UpdatedAt,
      ClosedAt = ticket.ClosedAt,
      UserId = ticket.UserId
    };

    return Ok(ticketDto);

  }
  [HttpPatch("{id}/close")]
  public async Task<ActionResult<TicketDto>> CloseTicket(int id)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (ticket == null)
    {
      return NotFound();
    }

    ticket.Status = "Closed";
    ticket.ClosedAt = DateTime.UtcNow;
    ticket.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    var ticketDto = new TicketDto
    {
      Id = ticket.Id,
      Title = ticket.Title,
      Description = ticket.Description,
      Status = ticket.Status,
      Priority = ticket.Priority,
      CreatedAt = ticket.CreatedAt,
      UpdatedAt = ticket.UpdatedAt,
      ClosedAt = ticket.ClosedAt,
      UserId = ticket.UserId
    };

    return Ok(ticketDto);
  }
  [HttpGet("admin/all")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTicketsForAdmin()
  {
    var tickets = await _context.Tickets
        .OrderByDescending(t => t.CreatedAt)
        .Select(t => new TicketDto
        {
          Id = t.Id,
          Title = t.Title,
          Description = t.Description,
          Status = t.Status,
          Priority = t.Priority,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt,
          ClosedAt = t.ClosedAt,
          UserId = t.UserId
        })
        .ToListAsync();

    return Ok(tickets);
  }

}