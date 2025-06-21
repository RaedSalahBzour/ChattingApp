using System.ComponentModel.DataAnnotations;

namespace ChattingAppAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
