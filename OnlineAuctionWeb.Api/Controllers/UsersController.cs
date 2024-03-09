using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(page, pageSize);
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var user = await _userService.CreateAsync(userDto);
            return Ok(user);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var user = await _userService.UpdateAsync(id, userDto);
            return Ok(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }   
    }
}
