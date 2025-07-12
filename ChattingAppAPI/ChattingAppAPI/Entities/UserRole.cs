using Microsoft.AspNetCore.Identity;

namespace ChattingAppAPI.Entities;

public class UserRole : IdentityUserRole<int>
{
    public AppUser User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}
