using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using ChattingAppAPI.Migrations;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser)
    {
        var query = _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
        if (isCurrentUser) query = query.IgnoreQueryFilters();
        return await query.SingleOrDefaultAsync();

    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        if (userParams.Gender == "male" || userParams.Gender == "female")
            query = query.Where(u => u.Gender == userParams.Gender);

        query = userParams.OrderBy switch
        {
            "createdAt" => query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        var oldestAllowedDob = DateOnly
                .FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var youngestAllowedDob = DateOnly
            .FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        query = query.Where(u => u.DateOfBirth >= oldestAllowedDob
        && u.DateOfBirth <= youngestAllowedDob);
        return await PagedList<MemberDto>
            //convert it from data base to MemberDto
            .CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            , userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);

    }


    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public async Task<AppUser?> GetUserByPhotoId(int photoId)
    {
        return await _context.Users
                   .Include(p => p.Photos)
                   .IgnoreQueryFilters()
                   .Where(p => p.Photos.Any(p => p.Id == photoId))
                   .FirstOrDefaultAsync();
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
