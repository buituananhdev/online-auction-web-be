using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Infrastructure.Authorize;

namespace OnlineAuctionWeb.Api.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a paginated list of users based on optional search and filter criteria.
        /// </summary>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <param name="searchQuery">Optional search query to filter users by name or email.</param>
        /// <param name="status">Optional status filter to filter users by their status.</param>
        /// <returns>Returns a paginated list of users.</returns>
        [HttpGet]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? searchQuery = null, 
            [FromQuery] StatusEnum status = 0)
        {
            var users = await _userService.GetAllAsync(page, pageSize, searchQuery, status);
            return Ok(users);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>Returns the user with the specified ID.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto">The data for creating the new user.</param>
        /// <returns>Returns the created user.</returns>
        [HttpPost]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            await _userService.CreateAsync(userDto);
            return StatusCode(201);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="userDto">The updated user data.</param>
        /// <returns>Returns the updated user.</returns>
        [HttpPut]
        [Route("{id}")]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var user = await _userService.UpdateAsync(id, userDto);
            return Ok(user);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Returns a success message upon successful deletion.</returns>
        [HttpPatch]
        [Route("{id}")]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok(new { Message = "User deleted successfully." });
        }

        /// <summary>
        /// Get the current user.
        /// </summary>
        /// <returns>Returns current user information.</returns>
        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            return Ok(await _userService.GetMe());
        }

        /// <summary>
        ///  Change password.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            await _userService.ChangePassword(newPassword);
            return Ok();
        }
    }
}
