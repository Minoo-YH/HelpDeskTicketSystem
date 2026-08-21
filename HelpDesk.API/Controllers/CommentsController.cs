using System.Security.Claims;
using HelpDesk.API.Data;
using HelpDesk.API.DTOs.Comments;
using HelpDesk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/tickets/{ticketId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public CommentsController(ApplicationDbContext context)
  {
    _context = context;
  }

  [HttpPost]
  public async Task<ActionResult<CommentDto>> AddComment(
      int ticketId,
      CreateCommentDto dto)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    var roleClaim = User.FindFirst(ClaimTypes.Role);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == ticketId);

    if (ticket == null)
    {
      return NotFound("Ticket not found.");
    }

    var isAdmin = roleClaim?.Value == "Admin";

    if (ticket.UserId != userId && !isAdmin)
    {
      return Forbid();
    }

    var comment = new TicketComment
    {
      Content = dto.Content,
      CreatedAt = DateTime.UtcNow,
      TicketId = ticketId,
      UserId = userId
    };

    _context.TicketComments.Add(comment);
    await _context.SaveChangesAsync();

    var result = new CommentDto
    {
      Id = comment.Id,
      Content = comment.Content,
      CreatedAt = comment.CreatedAt,
      TicketId = comment.TicketId,
      UserId = comment.UserId
    };

    return Ok(result);
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
      int ticketId)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    var roleClaim = User.FindFirst(ClaimTypes.Role);

    if (userIdClaim == null ||
        !int.TryParse(userIdClaim.Value, out var userId))
    {
      return Unauthorized();
    }

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == ticketId);

    if (ticket == null)
    {
      return NotFound("Ticket not found.");
    }

    var isAdmin = roleClaim?.Value == "Admin";

    if (ticket.UserId != userId && !isAdmin)
    {
      return Forbid();
    }

    var comments = await _context.TicketComments
        .Where(c => c.TicketId == ticketId)
        .OrderBy(c => c.CreatedAt)
        .Select(c => new CommentDto
        {
          Id = c.Id,
          Content = c.Content,
          CreatedAt = c.CreatedAt,
          TicketId = c.TicketId,
          UserId = c.UserId
        })
        .ToListAsync();

    return Ok(comments);
  }
}