using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OnlineAuctionWeb.Application.Services
{
    public interface ICurrentUserService
    {
        public int UserId { get; }
        public string Email { get; }
        public string Role { get; }
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int UserId => int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("ID"));

        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    }
}
