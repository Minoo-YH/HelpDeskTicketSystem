namespace HelpDesk.API.Services.AI;

public class GeminiResponse
{
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string SuggestedResponse { get; set; } = string.Empty;
}