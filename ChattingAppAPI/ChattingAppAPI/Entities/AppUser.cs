using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChattingAppAPI.Entities;

public class AppUser : IdentityUser<int>
{
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
    public List<UserLike> LikedByUsers { get; set; } = [];
    public List<UserLike> LikedUsers { get; set; } = [];
    public List<Message> MessageSent { get; set; } = [];
    public List<Message> MessageReceived { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];


}
