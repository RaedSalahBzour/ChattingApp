using AutoMapper;
using ChattingAppAPI.Data;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChattingAppAPI.Controllers;

public class AccountController(ApplicationDbContext context, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userExists(registerDto.Username)) return BadRequest("username is taken");
        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.GenerateToken(user),
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.SingleOrDefault(p => p.IsMain)?.Url
        };

    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await context.Users.Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.Username.ToLower());
        if (user is null || user.UserName.ToLower() != loginDto.Username.ToLower()) return Unauthorized("invalid username!");
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password!");

        }
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.GenerateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url

        };
    }
    private async Task<bool> userExists(string username)
    {
        return await context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }

}
