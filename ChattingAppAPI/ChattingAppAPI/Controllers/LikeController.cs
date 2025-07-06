using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChattingAppAPI.Controllers;

public class LikeController(ILikeRepository likeRepository) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest("you cannot like your self");
        var existingLike = await likeRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike == null)
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
            };
            likeRepository.AddLike(like);
        }
        else
        {
            likeRepository.DeleteLike(existingLike);
        }
        if (await likeRepository.SaveChanges()) return Ok();
        return BadRequest("failed to updated like");
    }
    [HttpGet("list")]
    public async Task<ActionResult> GetCurrentUserLikeIds()
    {
        return Ok(await likeRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }
    [HttpGet]
    public async Task<ActionResult> GetUserLikes([FromQuery] LikeParams likeParams)
    {
        likeParams.UserId = User.GetUserId();
        var users = await likeRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }
}
