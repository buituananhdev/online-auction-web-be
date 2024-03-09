﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;

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
        /// <returns>Returns the result of the registration process.</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto payload)
        {
            var result = await _authService.Register(payload);
            return Ok(result);
        }
    }
}
