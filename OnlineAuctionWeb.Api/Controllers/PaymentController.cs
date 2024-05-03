using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Infrastructure.Authorize;

namespace OnlineAuctionWeb.Api.Controllers
{
    /// <summary>
    /// Controller handling payment-related operations.
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Retrieves all payment records.
        /// </summary>
        /// <param name="pageNumber">Page number of the results (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <param name="searchQuery">Optional search query.</param>
        /// <returns>List of payment records.</returns>
        //[RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        [HttpGet]
        public async Task<IActionResult> GetAllPaymentAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchQuery = null
        )
        {
            var result = await _paymentService.GetAllAsync(pageNumber, pageSize, searchQuery);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new payment record.
        /// </summary>
        /// <param name="createPaymentDto">Payment details.</param>
        /// <returns>ActionResult indicating success or failure.</returns>
        //[RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Buyer })]
        [HttpPost]
        public async Task<IActionResult> CreatePaymentRecordAsync(CreatePaymentDto createPaymentDto)
        {
            await _paymentService.CreatePaymentAsync(createPaymentDto);
            return Ok();
        }
    }
}