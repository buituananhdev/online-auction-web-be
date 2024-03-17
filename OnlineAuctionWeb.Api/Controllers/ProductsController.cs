using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves all products with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>Returns a paginated list of products.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>Returns the product with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">The data for creating the product.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateProductDto productDto)
        {
            await _productService.CreateAsync(productDto);
            return StatusCode(201);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="productDto">The updated data for the product.</param>
        /// <returns>Returns the updated product.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProductDto productDto)
        {
            var result = await _productService.UpdateAsync(id, productDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
