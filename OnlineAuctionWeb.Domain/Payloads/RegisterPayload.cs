using OnlineAuctionWeb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Payloads
{
    public class RegisterPayload
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public RoleEnum? Role { get; set; }

    }
}
