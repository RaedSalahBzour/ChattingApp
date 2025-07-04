using System.Security.Claims;

namespace ChattingAppAPI.Extensions;

public static class ClaimsPrincipalExtenstions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name)
        ?? throw new Exception("cannot get name from token");
    }
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("cannot get Id from token")
        );
    }
}
