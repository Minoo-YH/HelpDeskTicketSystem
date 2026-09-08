using HelpDesk.API.Models;
using Microsoft.Extensions.Options;

namespace HelpDesk.API.Services.AI;

public class GeminiService
{
  private readonly GeminiSettings _settings;

  public GeminiService(IOptions<GeminiSettings> settings)
  {
    _settings = settings.Value;
  }

  public async Task<string> AnalyzeTicketAsync(string ticketText)
  {
    return await Task.FromResult("AI service is ready.");
  }
}