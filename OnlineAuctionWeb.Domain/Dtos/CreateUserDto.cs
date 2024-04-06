using OnlineAuctionWeb.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateUserDto
    {
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 32 characters")]
        public string Password { get; set; }

        public string Avatar { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [EnumDataType(typeof(RoleEnum), ErrorMessage = "Invalid role")]
        public RoleEnum Role { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(StatusEnum), ErrorMessage = "Invalid status")]
        public StatusEnum Status { get; set; }
    }
}
