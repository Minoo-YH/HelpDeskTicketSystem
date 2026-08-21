using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.Comments;

public class CreateCommentDto
{
  [Required]
  public string Content { get; set; } = string.Empty;
}