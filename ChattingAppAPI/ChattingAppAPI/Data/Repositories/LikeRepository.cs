using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChattingAppAPI.Data.Repositories;

public class LikeRepository(ApplicationDbContext context, IMapper mapper) : ILikeRepository
{
    public void AddLike(UserLike like)
    {
        context.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Remove(like);
    }

    public async Task<IEnumerable<int>>? GetCurrentUserLikeIds(int CurrentUserId)
    {
        return await context.Likes
            .Where(l => l.SourceUserId == CurrentUserId)
            .Select(l => l.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike>? GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes
             .FindAsync(sourceUserId, targetUserId);

    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikeParams likeParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;
        switch (likeParams.Predicate)
        {
            case "liked":
                query = likes.Where(l => l.SourceUserId == likeParams.UserId)
                    .Select(l => l.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likes.Where(l => l.TargetUserId == likeParams.UserId)
                   .Select(l => l.SourceUser)
                   .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(likeParams.UserId);
                query = likes
                    .Where(l => l.TargetUserId == likeParams.UserId && likeIds.Contains(l.SourceUserId))
                    .Select(l => l.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<MemberDto>
            .CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
