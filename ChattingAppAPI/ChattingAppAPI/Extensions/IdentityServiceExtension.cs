using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChattingAppAPI.Extensions;

public static class IdentityServiceExtension
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        //JwtBearerDefaults.AuthenticationScheme is just a shortcut for the string "Bearer"
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var TokenKey = configuration["TokenKey"]
                ?? throw new Exception("Token key not found");
                // Define how the JWT tokens will be validated
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Require the token to be signed with a valid key
                    ValidateIssuerSigningKey = true,

                    // Use our secret key to validate the signature
                    //signature means:
                    //This token was created by a trusted server and was not changed or faked.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),

                    // Don't check the token's issuer (e.g., "auth.myapi.com")
                    ValidateIssuer = false,

                    // Don't check the token's audience (e.g., "myapp-client")
                    ValidateAudience = false,
                };
            });
        return services;
    }
}
