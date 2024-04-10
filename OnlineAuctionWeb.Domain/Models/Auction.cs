using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Auction : BaseDomainEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public ConditionEnum Condition { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean CanReturn { get; set; }
        public ProductStatusEnum ProductStatus { get; set; }
        public Int32 ViewCount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<AuctionMedia> ProductMedias { get; set; }
        public List<Bid> Bids { get; set; }
    }
}
