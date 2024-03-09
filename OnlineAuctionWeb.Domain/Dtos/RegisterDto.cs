using OnlineAuctionWeb.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public RoleEnum? Role { get; set; }
    }
}
