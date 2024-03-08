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
            try
            {
                var tokenPayload = await _authService.Login(payload.Email, payload.Password);
                return Ok(tokenPayload);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Err", ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPayload payload)
        {
            try
            {
                var result = await _authService.Register(payload);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
