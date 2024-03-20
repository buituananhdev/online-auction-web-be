using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;

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
        /// Retrieves all auctions with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>Returns a paginated list of auctions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _AuctionService.GetAllAsync(pageNumber, pageSize);
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
            await _AuctionService.CreateAsync(AuctionDto);
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
    }
}
