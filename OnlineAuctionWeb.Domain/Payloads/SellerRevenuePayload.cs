using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Payloads
{
    public class SellerRevenuePayload
    {
        public int TotalAuctions { get; set; }
        public int TotalSoldAuctions { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
