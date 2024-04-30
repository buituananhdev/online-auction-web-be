using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Models
{
    public class AuctionMedia
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; }
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
    }
}
