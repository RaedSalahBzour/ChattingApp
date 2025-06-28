using AutoMapper;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChattingAppAPI.Controllers;

[Authorize]
public class UserController(IUserRepository userRepository, IMapper mapper,
    IPhotoService photoService) : BaseApiController
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
        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if (user is null) return NotFound("user not found");
        mapper.Map(dto, user);
        if (await userRepository.SaveAllAsync())
            return NoContent();
        return BadRequest("faild tp update the user");
    }
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if (user is null) return BadRequest("cannot update user");
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error is not null)
        {
            return BadRequest(result.Error.Message);
        }
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
        };
        user.Photos.Add(photo);
        if (await userRepository.SaveAllAsync())
            return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
        return BadRequest("something went wrong while adding the photo");
    }
}
