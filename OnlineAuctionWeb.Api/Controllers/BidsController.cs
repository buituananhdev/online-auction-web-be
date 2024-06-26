﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Infrastructure.Authorize;

namespace OnlineAuctionWeb.Api.Controllers
{
    /// <summary>
    /// Controller handling bids related operations.
    /// </summary>
    [Route("api/bids")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _bidService;

        public BidsController(IBidService bidService)
        {
            _bidService = bidService;
        }

        /// <summary>
        /// Retrieves a list of bids.
        /// </summary>
        /// <param name="page">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>List of bids.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> GetBids([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var bids = await _bidService.GetAllAsync(page, pageSize);
            return Ok(bids);
        }

        /// <summary>
        /// Creates a new bid.
        /// </summary>
        /// <param name="bidDto">Bid information.</param>
        /// <returns>Status 201 if successful, or status 400 if bad request.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Buyer })]
        public async Task<IActionResult> CreateBid([FromBody] CreateBidDto bidDto)
        {
            var result = await _bidService.CreateAsync(bidDto);
            return Ok(result);
        }
    }
}
