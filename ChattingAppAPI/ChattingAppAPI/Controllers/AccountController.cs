using AutoMapper;
using ChattingAppAPI.Data;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChattingAppAPI.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userExists(registerDto.Username)) return BadRequest("username is taken");
        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.GenerateToken(user),
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.SingleOrDefault(p => p.IsMain)?.Url
        };

    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await userManager.Users.Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.Username.ToUpper());
        if (user is null || user.NormalizedUserName
            != loginDto.Username.ToUpper() || user.UserName is null)
            return Unauthorized("invalid username!");
        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized();
        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.GenerateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url

        };
    }
    private async Task<bool> userExists(string username)
    {
        return await userManager.Users
            .AnyAsync(u => u.NormalizedUserName == username.ToUpper());
    }

}
