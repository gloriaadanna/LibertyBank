using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers;

public static class TokenHelper
{
    public static string GenerateToken(IConfiguration configuration, Guid customerId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = configuration["JWT:Secret"];
        var key = System.Text.Encoding.ASCII.GetBytes(secretKey!);
            
        // Security Token Descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Name,customerId.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(4),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}