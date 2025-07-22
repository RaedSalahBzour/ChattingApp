using ChattingAppAPI.Data;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Middlewares;
using ChattingAppAPI.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader()
.AllowAnyMethod().AllowCredentials()
.WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scpoe = app.Services.CreateScope();
var sercices = scpoe.ServiceProvider;
try
{
    var context = sercices.GetRequiredService<ApplicationDbContext>();
    var userManager = sercices.GetRequiredService<UserManager<AppUser>>();
    var roleManager = sercices.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("Delete From [Connections]");
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = sercices.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
app.Run();
