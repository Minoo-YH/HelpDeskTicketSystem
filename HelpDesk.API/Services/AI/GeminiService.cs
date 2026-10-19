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
    if (string.IsNullOrWhiteSpace(_settings.ApiKey))
      throw new InvalidOperationException("Gemini API Key is missing.");

    var prompt = $"""
            Analyze this helpdesk ticket:
            {ticketText}

            Return JSON with:
            - category: Hardware, Software, Account, Network, or Other
            - priority: Low, Medium, High, or Critical
            - suggestedResponse: a helpful response to the user
            """;

    var requestBody = new
    {
      contents = new[]
        {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
      generationConfig = new
      {
        responseMimeType = "application/json"
      }
    };

    var url =
        $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent";

    using var request = new HttpRequestMessage(HttpMethod.Post, url);
    request.Headers.Add("x-goog-api-key", _settings.ApiKey);
    request.Content = new StringContent(
        JsonSerializer.Serialize(requestBody),
        Encoding.UTF8,
        "application/json");

    using var response = await _httpClient.SendAsync(request);
    if (!response.IsSuccessStatusCode)
    {
      var errorBody = await response.Content.ReadAsStringAsync();

      Console.WriteLine(
          $"Gemini API Error {(int)response.StatusCode}: {errorBody}");

      throw new HttpRequestException(
          $"Gemini API returned {(int)response.StatusCode}");
    }

    using var json = JsonDocument.Parse(
        await response.Content.ReadAsStringAsync());

    return json.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString() ?? string.Empty;
  }
}