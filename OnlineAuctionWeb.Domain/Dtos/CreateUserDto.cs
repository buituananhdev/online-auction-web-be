using OnlineAuctionWeb.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string Password { get; set; }

        [EnumDataType(typeof(RoleEnum), ErrorMessage = "Invalid role")]
        public RoleEnum Role { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public StatusEnum IsActive { get; set; }
    }
}
