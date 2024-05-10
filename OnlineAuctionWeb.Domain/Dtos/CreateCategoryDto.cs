using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}