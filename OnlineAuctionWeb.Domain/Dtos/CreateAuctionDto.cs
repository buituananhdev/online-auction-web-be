using System;
using System.ComponentModel.DataAnnotations;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateAuctionDto
    {
        [Required(ErrorMessage = "Auction Name is required")]
        public string ProductName { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Condition is required")]
        public ConditionEnum Condition { get; set; } = ConditionEnum.New;

        [Required(ErrorMessage = "Starting Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Starting Price must be greater than or equal to 0")]
        public decimal StartingPrice { get; set; }

        [Required(ErrorMessage = "End Time is required")]
        [FutureDate(ErrorMessage = "End Time must be a future date")]
        public DateTime EndTime { get; set; }

        public bool CanReturn { get; set; } = false;

        public ProductStatusEnum ProductStatus { get; set; } = ProductStatusEnum.Available;

        [Required(ErrorMessage = "Seller ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Seller ID must be greater than 0")]
        public int SellerId { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryId { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime inputValue = Convert.ToDateTime(value);
            return inputValue > DateTime.Now;
        }
    }
}
