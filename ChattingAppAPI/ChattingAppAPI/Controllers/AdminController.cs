﻿using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using ChattingAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChattingAppAPI.Controllers;

public class AdminController(UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork, IPhotoService photoService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.OrderBy
            (x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();
        return Ok(users);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("select at least one role");
        var selectedRoles = roles.Split(',').ToArray();
        var user = await userManager.FindByNameAsync(username);
        if (user == null) return NotFound("user not found");
        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager
            .AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded)
            return BadRequest("fail to add roles");
        result = await userManager
            .RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded)
            return BadRequest("fail to remove roles");
        return Ok(await userManager.GetRolesAsync(user));
    }
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("approve/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not get photo from database");
        photo.IsApproved = true;
        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        if (user == null) return BadRequest("Could not get user from database");
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
        await unitOfWork.Complete();
        return Ok();
    }
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not get photo from db");
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        await unitOfWork.Complete();
        return Ok();
    }
}
