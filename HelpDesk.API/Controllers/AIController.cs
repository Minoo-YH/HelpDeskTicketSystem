using HelpDesk.API.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly GeminiService _geminiService;

    public AIController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(
        [FromBody] GeminiRequest request)
    {
        var result = await _geminiService.AnalyzeTicketAsync(
            request.TicketText);

        return Ok(result);
    }
}