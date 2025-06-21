using ChattingAppAPI.Data;
using ChattingAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Controllers;

[Authorize]
public class UserController(ApplicationDbContext context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await context.Users.ToListAsync());
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        AppUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
}
