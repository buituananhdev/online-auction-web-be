using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Infrastructure.Authorize;
using System.Threading.Tasks;

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
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync(CreateCategoryDto categoryDto)
        {
            await _categoryService.CreateAsync(categoryDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Retrieves all categories with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>Returns a paginated list of categories.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="categoryDto">The updated data for the category.</param>
        /// <returns>Returns the updated category.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            var result = await _categoryService.UpdateAsync(id, categoryDto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>Returns the status code indicating success.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Update the status of a category by its unique identifier.
        /// </summary>
        /// <remarks>
        /// This endpoint allows updating the status of a category identified by its unique identifier.
        /// </remarks>
        /// <param name="id">The unique identifier of the category.</param>
        /// <param name="status">The new status to be assigned to the category.</param>
        /// <response code="200">Returns success if the status is updated successfully.</response>
        /// <response code="404">If the category with the specified identifier is not found.</response>
        /// <response code="400">If the provided status is invalid or any other request-related errors occur.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RolesAuthorize(RequiredRoles = new RoleEnum[] { RoleEnum.Admin })]
        public async Task<IActionResult> ChangeStatusAsync(int id, StatusEnum status)
        {
            await _categoryService.ChangeStatusAsync(id, status);
            return Ok();
        }
    }
}
