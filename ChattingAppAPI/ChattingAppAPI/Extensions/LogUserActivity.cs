using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChattingAppAPI.Extensions;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (context.HttpContext.User.Identity?.IsAuthenticated is not true) return;
        //gives the context of before the execution of the action
        //var username1 = context.HttpContext.User.GetUserName();
        //gives the context of after the execution of the action
        var userId = resultContext.HttpContext.User.GetUserId();
        var repo = resultContext.HttpContext
            .RequestServices.GetRequiredService<IUserRepository>();
        var user = await repo.GetUserByIdAsync(userId);
        if (user == null) return;
        user.LastActive = DateTime.UtcNow;
        await repo.SaveAllAsync();
    }
}
