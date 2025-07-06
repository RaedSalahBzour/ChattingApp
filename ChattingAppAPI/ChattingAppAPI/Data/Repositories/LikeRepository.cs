using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
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

    public async Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
    {
        var likes = context.Likes.AsQueryable();
        switch (predicate)
        {
            case "liked":
                return await likes.Where(l => l.SourceUserId == userId)
                    .Select(l => l.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();

            case "likedBy":
                return await likes.Where(l => l.TargetUserId == userId)
                   .Select(l => l.SourceUser)
                   .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                   .ToListAsync();

            default:
                var likeIds = await GetCurrentUserLikeIds(userId);
                return await likes
                    .Where(l => l.TargetUserId == userId && likeIds.Contains(l.SourceUserId))
                    .Select(l => l.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();

        }
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
