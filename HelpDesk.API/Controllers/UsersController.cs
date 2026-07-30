using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly JwtService _jwtService;

  public UsersController(
      ApplicationDbContext context,
      JwtService jwtService)
  {
    _context = context;
    _jwtService = jwtService;
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

    var token = _jwtService.GenerateToken(user);

    return Ok(token);
  }

  [Authorize]
  [HttpGet("profile")]
  public IActionResult GetProfile()
  {
    return Ok(new
    {
      Message = "You are authenticated!"
    });
  }
}