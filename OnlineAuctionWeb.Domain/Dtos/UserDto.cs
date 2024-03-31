using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class UserDto : BaseDomainEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; }
        public string Avatar { get; set; }
        public RoleEnum Role { get; set; }
        public StatusEnum Status { get; set; }
        public AvarageRatingPayload ratings { get; set; }
    }
}
