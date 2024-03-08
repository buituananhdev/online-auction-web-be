using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;
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
        public int Role { get; set; }
        public StatusEnum IsActive { get; set; }
    }
}
