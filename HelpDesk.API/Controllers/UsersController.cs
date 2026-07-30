using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
      CreatedAt = DateTime.UtcNow
    };

    _context.Users.Add(user);

    await _context.SaveChangesAsync();

    return Ok("User registered successfully.");
  }
  [HttpPost("login")]
  public IActionResult Login(LoginRequest request)
  {
    var user = _context.Users.FirstOrDefault(x => x.Email == request.Email);

    if (user == null)
    {
      return Unauthorized("Invalid email or password.");
    }

    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
        request.Password,
        user.PasswordHash);

    if (!isPasswordValid)
    {
      return Unauthorized("Invalid email or password.");
    }

    return Ok("Login successful.");
  }
}