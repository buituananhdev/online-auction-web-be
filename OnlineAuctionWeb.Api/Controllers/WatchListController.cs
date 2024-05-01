using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/watchlist")]
    [ApiController]
    public class WatchListController : ControllerBase
    {
        private readonly IWatchListService _watchListService;
        public WatchListController(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        /// <summary>
        /// Add auction to watch list
        /// </summary>
        /// <param name="createWatchListDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddToWatchListAsync(CreateWatchListDto createWatchListDto)
        {
            await _watchListService.AddToWatchListAsync(createWatchListDto);
            return Ok();
        }

        /// <summary>
        /// Get watch list by user ID
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        /// <response code="200">Returns the watch list</response>
        /// <response code="401">Invalid token</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<IActionResult> GetWatchListByUserIDAsync(int pageNumber = 1, int pageSize = 10, string? searchQuery = null, WatchListTypeEnum? type = null)
        {
            var result = await _watchListService.GetWatchListByUserIDAsync(pageNumber, pageSize, searchQuery, type);
            return Ok(result);
        }
    }
}
