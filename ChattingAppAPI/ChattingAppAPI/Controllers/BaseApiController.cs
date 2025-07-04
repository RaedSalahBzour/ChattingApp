using ChattingAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ChattingAppAPI.Controllers;
[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
}
