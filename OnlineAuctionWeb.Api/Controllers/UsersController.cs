using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
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
        /// Gets a paginated list of users.
        /// </summary>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>Returns a paginated list of users.</returns>
        [HttpGet]
        //[RolesAuthorize(RequiredRoles = new int[] { (int)RoleEnum.Admin })]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(page, pageSize);
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
        [RolesAuthorize(RequiredRoles = new int[] { (int)RoleEnum.Admin })]
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
        [RolesAuthorize(RequiredRoles = new int[] { (int)RoleEnum.Admin })]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
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
        [RolesAuthorize(RequiredRoles = new int[] { (int)RoleEnum.Admin })]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok(new { Message = "User deleted successfully." });
        }
    }
}
