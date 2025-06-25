using AutoMapper;
using ChattingAppAPI.Data;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository) : BaseApiController
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
    //[HttpPut("{id:int}")]
    //public async Task<ActionResult<AppUser>> UpdateUser(int id, AppUser appUser)
    //{
    //    AppUser? user = await userRepository.GetUserByIdAsync(id);
    //    if (user is null)
    //        return NotFound();

    //    userRepository.Update(appUser);
    //    return Ok(user);
    //}
}
