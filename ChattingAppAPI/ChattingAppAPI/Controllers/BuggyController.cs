using ChattingAppAPI.Data;
using ChattingAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattingAppAPI.Controllers;

public class BuggyController : BaseApiController
{
    private readonly ApplicationDbContext _context;
    public BuggyController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = _context.Users.Find(-1);

        if (thing == null) return NotFound("not found bro");

        return Ok(thing);
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {
        var thing = _context.Users.Find(-1);

        if (thing is null)
        {
            throw new Exception("Something went wrong,internal server error 500");
        }

        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("badddddddddddd");
    }
}

