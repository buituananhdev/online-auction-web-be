﻿using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _AuctionService;
        public AuctionsController(IAuctionService AuctionService)
        {
            _AuctionService = AuctionService;
        }

        /// <summary>
        /// Retrieves a paginated list of auctions with optional filtering and search capabilities.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="searchQuery">Optional. A search query to filter auctions by name or description.</param>
        /// <param name="condition">Optional. Filters auctions by their condition. 1: New, 2: Open box, 3: Used</param>
        /// <param name="minCurrentPrice">Optional. Filters auctions by the minimum current price.</param>
        /// <param name="maxCurrentPrice">Optional. Filters auctions by the maximum current price.</param>
        /// <param name="minMaxPrice">Optional. Filters auctions by the minimum maximum price (reserve price).</param>
        /// <param name="maxMaxPrice">Optional. Filters auctions by the maximum maximum price (reserve price).</param>
        /// <param name="minEndTime">Optional. Filters auctions by the minimum end time.</param>
        /// <param name="maxEndTime">Optional. Filters auctions by the maximum end time.</param>
        /// <returns>Returns a paginated list of auctions based on the provided filters and pagination parameters.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchQuery = null,
            ConditionEnum? condition = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null)
        {
            var result = await _AuctionService.GetAllAsync(pageNumber, pageSize, searchQuery, condition, minCurrentPrice, maxCurrentPrice, minMaxPrice, maxMaxPrice, minEndTime, maxEndTime);
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
            var result = await _AuctionService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new auction.
        /// </summary>
        /// <param name="AuctionDto">The data for creating the auction.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAuctionDto AuctionDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            await _AuctionService.CreateAsync(AuctionDto, int.Parse(userId));
            return StatusCode(201);
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="id">The unique identifier of the auction to update.</param>
        /// <param name="AuctionDto">The updated data for the auction.</param>
        /// <returns>Returns the updated auction.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, AuctionDto AuctionDto)
        {
            var result = await _AuctionService.UpdateAsync(id, AuctionDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a auction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the auction to delete.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _AuctionService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedAuctions(int count)
        {
            await _AuctionService.SeedData(count);
            return StatusCode(201);
        }
    }
}
