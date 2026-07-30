using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.Password,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }
}