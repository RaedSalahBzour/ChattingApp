using System.Security.Claims;

namespace ChattingAppAPI.Extensions;

public static class ClaimsPrincipalExtenstions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("cannot get name from token");
    }
}
