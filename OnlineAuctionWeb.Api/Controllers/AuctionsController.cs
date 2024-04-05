using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Infrastructure.Hubs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly IHubContext<AuctionHub> _hubContext;

        public AuctionsController(IAuctionService auctionService, IHubContext<AuctionHub> hubContext)
        {
            _auctionService = auctionService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Retrieves a paginated list of auctions with optional filtering and search capabilities.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="searchQuery">Optional. A search query to filter auctions by name or description.</param>
        /// <param name="conditions">Optional. Filters auctions by their condition. 1: New, 2: Open box, 3: Used</param>
        /// <param name="minCurrentPrice">Optional. Filters auctions by the minimum current price.</param>
        /// <param name="maxCurrentPrice">Optional. Filters auctions by the maximum current price.</param>
        /// <param name="minMaxPrice">Optional. Filters auctions by the minimum maximum price (reserve price).</param>
        /// <param name="maxMaxPrice">Optional. Filters auctions by the maximum maximum price (reserve price).</param>
        /// <param name="minEndTime">Optional. Filters auctions by the minimum end time.</param>
        /// <param name="maxEndTime">Optional. Filters auctions by the maximum end time.</param>
        /// <param name="categoryIds">Optional. Filters auctions by category.</param>
        /// <returns>Returns a paginated list of auctions based on the provided filters and pagination parameters.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchQuery = null,
            string? conditions = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null,
            string? categoryIds = null)
        {
            var result = await _auctionService.GetAllAsync(pageNumber, pageSize, searchQuery, conditions, minCurrentPrice, maxCurrentPrice, minMaxPrice, maxMaxPrice, minEndTime, maxEndTime, categoryIds);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific auction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the auction.</param>
        /// <returns>Returns the auction with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _auctionService.GetByIdAsync(id);
            await _hubContext.Clients.All.SendAsync("UserJoinAuctionAsync", id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new auction.
        /// </summary>
        /// <param name="auctionDto">The data for creating the auction.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAuctionDto auctionDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            await _auctionService.CreateAsync(auctionDto, int.Parse(userId));
            return StatusCode(201);
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="id">The unique identifier of the auction to update.</param>
        /// <param name="auctionDto">The updated data for the auction.</param>
        /// <returns>Returns the updated auction.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, AuctionDto auctionDto)
        {
            var result = await _auctionService.UpdateAsync(id, auctionDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an auction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the auction to delete.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _auctionService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Cancels an auction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the auction to cancel.</param>
        /// <param name="status">The new status to be assigned to the auction.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> ChangeStatusAsync(int id, ProductStatusEnum status)
        {
            await _auctionService.ChangeStatusAsync(id, status);
            return Ok();
        }

        /// <summary>
        /// Seeds the database with the specified number of auctions.
        /// </summary>
        /// <param name="count">The number of auctions to seed.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedAuctions(int count)
        {
            await _auctionService.SeedData(count);
            return StatusCode(201);
        }
    }
}
