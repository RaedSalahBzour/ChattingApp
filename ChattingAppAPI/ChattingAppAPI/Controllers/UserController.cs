using ChattingAppAPI.Data;
using ChattingAppAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ApplicationDbContext context) : ControllerBase
{
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
