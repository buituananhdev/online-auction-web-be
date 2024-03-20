using OnlineAuctionWeb.Domain.Common;

namespace OnlineAuctionWeb.Domain.Models
{
    public class WatchList : BaseDomainEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
    }
}
