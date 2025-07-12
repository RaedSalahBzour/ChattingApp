using ChattingAppAPI.Entities;

namespace ChattingAppAPI.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(AppUser user);
}
