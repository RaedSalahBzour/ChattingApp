using System.ComponentModel.DataAnnotations;

namespace ChattingAppAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "minimum length is 8 characters")]
        public string? Password { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? Coutry { get; set; }
        public string? KnownAs { get; set; }
    }
}
