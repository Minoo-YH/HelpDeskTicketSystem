using HelpDesk.API.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace HelpDesk.API.Services.AI;

public class GeminiService
{
  private readonly HttpClient _httpClient;
  private readonly GeminiSettings _settings;

  public GeminiService(
      HttpClient httpClient,
      IOptions<GeminiSettings> settings)
  {
    _httpClient = httpClient;
    _settings = settings.Value;
  }

  public async Task<string> AnalyzeTicketAsync(string ticketText)
  {
    return await Task.FromResult("AI service is ready.");
  }
}