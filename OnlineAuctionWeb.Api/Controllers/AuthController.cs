using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Domain.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Application.Services;

namespace OnlineAuctionWeb.Api.Controllers
{
    /// <summary>
    /// Controller for handling authentication operations.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="payload">The login payload.</param>
        /// <returns>Returns the authentication token payload upon successful login.</returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto payload)
        {
            var tokenPayload = await _authService.Login(payload);
            return Ok(tokenPayload);
        }

        /// <summary>
        /// Handles user registration.
        /// </summary>
        /// <param name="payload">The registration payload.</param>
        /// <returns>Returns created status code</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto payload)
        {
            await _authService.Register(payload);
            return StatusCode(201);
        }

        /// <summary>
        /// Authenticates user with Google.
        /// </summary>
        /// <param name="googleToken">The Google authentication token.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>Returns authentication result.</returns>
        [HttpPost]
        [Route("google")]
        public async Task<IActionResult> AuthWithGoogle([FromQuery] string googleToken, [FromQuery] RoleEnum role)
        {
            var result = await _authService.AuthWithGoogle(googleToken, role);
            return Ok(result);
        }
    }
}
