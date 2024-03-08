using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineAuctionWeb.Domain.Payloads;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineAuctionWeb.Application.Utils
{
    public class JwtUtil
    {
        private static SymmetricSecurityKey GetSymmetricSecurityKey(string secretKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        private static TokenPayload GenerateToken(int userId, string secretKey, double expirationDays)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSymmetricSecurityKey(secretKey);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("ID", userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                SigningCredentials = creds
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            var expiresIn = DateTimeOffset.UtcNow.AddDays(expirationDays);

            var token = new TokenPayload();
            token.UserId = userId;
            token.AccessToken = tokenHandler.WriteToken(jwtToken);
            token.ExpirationTime = expiresIn.ToUnixTimeSeconds();
            token.CreatedAt = DateTime.Now;
            return token;
        }

        public static TokenPayload GenerateAccessToken(int userId, IConfiguration configuration)
        {
            var secretKey = configuration.GetSection("JwtSettings:Secret").Value;
            var expirationTime = double.Parse(configuration.GetSection("JwtSettings:ExpirationTime").Value);
            return GenerateToken(userId, secretKey, expirationTime);
        }
    }
}
