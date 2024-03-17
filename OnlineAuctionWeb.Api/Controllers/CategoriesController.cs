using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain.Dtos;

namespace OnlineAuctionWeb.Api.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service to use.</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryDto">The data for creating the category.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCategoryDto categoryDto)
        {
            await _categoryService.CreateAsync(categoryDto);
            return StatusCode(201);
        }

        /// <summary>
        /// Retrieves all categories with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>Returns a paginated list of categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _categoryService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>Returns the category with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="categoryDto">The updated data for the category.</param>
        /// <returns>Returns the updated category.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, CategoryDto categoryDto)
        {
            var result = await _categoryService.UpdateAsync(id, categoryDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
