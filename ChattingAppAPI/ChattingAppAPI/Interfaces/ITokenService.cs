using ChattingAppAPI.Entities;

namespace ChattingAppAPI.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user);
}
