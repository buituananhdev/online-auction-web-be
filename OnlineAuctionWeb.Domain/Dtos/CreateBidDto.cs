using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateBidDto
    {
        public int AuctionId { get; set; }
        public decimal BidAmount { get; set; }
    }
}
