using System.Security.Claims;
using HelpDesk.API.Data;
using HelpDesk.API.DTOs.Tickets;
using HelpDesk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDesk.API.Services.AI;
using System.Text.Json;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly GeminiService _geminiService;

  public TicketsController(
      ApplicationDbContext context,
      GeminiService geminiService)
  {
    _context = context;
    _geminiService = geminiService;
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

    try
    {
      var aiJson = await _geminiService.AnalyzeTicketAsync(
          $"{ticket.Title}\n{ticket.Description}");

      var aiResult = JsonSerializer.Deserialize<GeminiResponse>(
          aiJson,
          new JsonSerializerOptions
          {
            PropertyNameCaseInsensitive = true
          });

      if (aiResult != null)
      {
        ticket.AiCategory = aiResult.Category;
        ticket.AiPriority = aiResult.Priority;
        ticket.AiSuggestedResponse = aiResult.SuggestedResponse;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Gemini analysis failed: {ex.Message}");
    }

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
      UserId = ticket.UserId,
      AiCategory = ticket.AiCategory,
      AiPriority = ticket.AiPriority,
      AiSuggestedResponse = ticket.AiSuggestedResponse
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
          UserId = t.UserId,
          AiCategory = t.AiCategory,
          AiPriority = t.AiPriority,
          AiSuggestedResponse = t.AiSuggestedResponse
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
          UserId = t.UserId,
          AiCategory = t.AiCategory,
          AiPriority = t.AiPriority,
          AiSuggestedResponse = t.AiSuggestedResponse
        })
        .ToListAsync();

    return Ok(tickets);
  }

}