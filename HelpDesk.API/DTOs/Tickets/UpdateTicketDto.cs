using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.Tickets;

public class UpdateTicketDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = "Medium";
}