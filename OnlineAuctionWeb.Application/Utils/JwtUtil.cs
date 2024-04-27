using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Payloads;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineAuctionWeb.Infrastructure.Utils
{
    public class JwtUtil
    {
        private static SymmetricSecurityKey GetSymmetricSecurityKey(string secretKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        private static TokenPayload GenerateToken(UserDto user, string secretKey, double expirationDays)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSymmetricSecurityKey(secretKey);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("ID", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                SigningCredentials = creds
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            var expiresIn = DateTimeOffset.UtcNow.AddDays(expirationDays);

            var token = new TokenPayload();
            token.UserId = user.Id;
            token.AccessToken = tokenHandler.WriteToken(jwtToken);
            token.ExpirationTime = expiresIn.ToUnixTimeSeconds();
            token.CreatedAt = DateTime.Now;
            token.Role = user.Role.ToString();
            return token;
        }

        public static TokenPayload GenerateAccessToken(UserDto user, IConfiguration configuration)
        {
            var secretKey = configuration.GetSection("JwtSettings:Secret").Value;
            var expirationTime = double.Parse(configuration.GetSection("JwtSettings:ExpirationTime").Value);
            return GenerateToken(user, secretKey, expirationTime);
        }
    }
}
