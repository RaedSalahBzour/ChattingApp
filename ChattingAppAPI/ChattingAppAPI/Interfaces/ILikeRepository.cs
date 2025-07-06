using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;

namespace ChattingAppAPI.Interfaces;

public interface ILikeRepository
{
    Task<UserLike>? GetUserLike(int sourceUserId, int targetUserId);
    Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId);
    Task<IEnumerable<int>>? GetCurrentUserLikeIds(int CurrentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
    Task<bool> SaveChanges();
}
