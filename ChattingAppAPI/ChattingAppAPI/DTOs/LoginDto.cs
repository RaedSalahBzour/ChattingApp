using System.ComponentModel.DataAnnotations;

namespace ChattingAppAPI.DTOs;

public class LoginDto
{
    [Required]
    public required string Username { get; set; } = string.Empty;
    [Required]
    public required string Password { get; set; } = string.Empty;
}
