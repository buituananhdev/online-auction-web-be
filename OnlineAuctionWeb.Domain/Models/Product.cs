using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Product : BaseDomainEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public ConditionEnum Condition { get; set; }
        public decimal StartingPrice { get; set; }
        public Boolean CanReturn { get; set; }
        public ProductStatusEnum ProductStatus { get; set; }
        public Int32 ViewCount { get; set; }
        public int SellerId { get; set; }
        public User Seller { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductMedia> ProductMedias { get; set; }
        public List<Bid> Bids { get; set; }
    }
}
