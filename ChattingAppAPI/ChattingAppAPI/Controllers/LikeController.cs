using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChattingAppAPI.Controllers;

public class LikeController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest("you cannot like your self");
        var existingLike = await unitOfWork.LikeRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike == null)
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
            };
            unitOfWork.LikeRepository.AddLike(like);
        }
        else
        {
            unitOfWork.LikeRepository.DeleteLike(existingLike);
        }
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("failed to updated like");
    }
    [HttpGet("list")]
    public async Task<ActionResult> GetCurrentUserLikeIds()
    {
        return Ok(await unitOfWork.LikeRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }
    [HttpGet]
    public async Task<ActionResult> GetUserLikes([FromQuery] LikeParams likeParams)
    {
        likeParams.UserId = User.GetUserId();
        var users = await unitOfWork.LikeRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }
}
