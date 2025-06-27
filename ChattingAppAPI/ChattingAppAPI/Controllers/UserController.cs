using AutoMapper;
using ChattingAppAPI.Data;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChattingAppAPI.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        return Ok(users);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        AppUser? user = await userRepository.GetUserByIdAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
    [HttpPut()]
    public async Task<ActionResult<AppUser>> UpdateUser(MemberUpdateDto dto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username == null) return NotFound("no username found in token");
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user is null) return NotFound("user not found");
        mapper.Map(dto, user);
        if (await userRepository.SaveAllAsync())
            return NoContent();
        return BadRequest("faild tp update the user");
    }
}
