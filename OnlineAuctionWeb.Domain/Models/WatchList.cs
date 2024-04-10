using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class WatchList : BaseDomainEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        public WatchListTypeEnum Type { get; set; }
    }
}
