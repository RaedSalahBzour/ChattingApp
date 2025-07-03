using ChattingAppAPI.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChattingAppAPI.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    public required string UserName { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public string KnownAs { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string? Introduction { get; set; }
    public string? Gender { get; set; }
    public string? Interests { get; set; }
    public string? LookingFor { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<Photo> Photos { get; set; } = [];

    public int CalculateAge()
    {
        return DateOfBirth.CalculateAge();
    }
}
