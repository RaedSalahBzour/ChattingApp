using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Helpers;

namespace ChattingAppAPI.Interfaces;

public interface ILikeRepository
{
    Task<UserLike>? GetUserLike(int sourceUserId, int targetUserId);
    Task<PagedList<MemberDto>> GetUserLikes(LikeParams likeParams);
    Task<IEnumerable<int>>? GetCurrentUserLikeIds(int CurrentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}
