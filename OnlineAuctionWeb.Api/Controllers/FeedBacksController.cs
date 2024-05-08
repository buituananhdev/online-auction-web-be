using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedBacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedBacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Create a new feedback
        /// </summary>
        /// <param name="createFeedBackDto">The feedback to create</param>
        /// <returns></returns>
        /// <response code="201">Returns when the feedback is created</response>
        /// <response code="400">Returns when the feedback is invalid</response>
        /// <response code="500">Returns when there is an internal server error</response>
        /// <response code="401">Returns when the user is not authenticated</response>
        /// <response code="403">Returns when the user is not authorized</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync(CreateFeedBackDto createFeedBackDto)
        {
            await _feedbackService.CreateAsync(createFeedBackDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Get feedbacks by user id
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns></returns>
        /// <response code="200">Returns when the feedbacks are found</response>
        /// <response code="400">Returns when the user id is invalid</response>
        /// <response code="500">Returns when there is an internal server error</response>
        /// <response code="401">Returns when the user is not authenticated</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAsync(int id)
        {
            var feedbacks = await _feedbackService.GetFeedBackByUserId(id);
            return Ok(feedbacks);
        }
    }
}
