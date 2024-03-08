using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Payloads
{
    public class TokenPayload
    {
        public int UserId { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public long? ExpirationTime { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
