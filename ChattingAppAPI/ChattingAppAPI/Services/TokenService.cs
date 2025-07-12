using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChattingAppAPI.Services;

public class TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    : ITokenService
{
    public async Task<string> GenerateToken(AppUser user)
    {
        var tokenKey = configuration["tokenKey"]
            ?? throw new Exception("can not access token key from appsettings!");
        //because SHA-512 generates a 512-bit (64-byte) hash. so it must at least be 64
        if (tokenKey.Length < 64) throw new Exception("your token key must be longer");
        // create a security key from the secret to use for signing the token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        // create a list of claims (user information) to include in the JWT
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new(ClaimTypes.Name,user.UserName),
        };
        // to specify how to sign the token using the key and algorithm
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        // defines what will be inside the JWT and how it will be signed
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            // tell the system how to create a secure signature for the token
            SigningCredentials = credentials,

        };
        // create a handler that can generate and write JWT tokens
        var tokenHandler = new JwtSecurityTokenHandler();
        // generate the JWT based on the descriptor (claims, expiration, signing)
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // convert the token to a string that can be sent to the client
        return tokenHandler.WriteToken(token);

    }
}
