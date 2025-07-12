using Microsoft.AspNetCore.Identity;

namespace ChattingAppAPI.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
