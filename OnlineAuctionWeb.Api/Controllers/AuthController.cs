using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Payloads;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload payload)
        {
            var tokenPayload = await _authService.Login(payload.Email, payload.Password);
            return Ok(tokenPayload);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPayload payload)
        {
            var result = await _authService.Register(payload);
            return Ok(result);
        }
    }
}
