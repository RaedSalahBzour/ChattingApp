using ChattingAppAPI.Data;
using ChattingAppAPI.Data.Repositories;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using ChattingAppAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Extensions;

public static class ApplicationServiceExtentions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDbContext<ApplicationDbContext>(options =>
       {
           options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
       });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        return services;
    }
}
