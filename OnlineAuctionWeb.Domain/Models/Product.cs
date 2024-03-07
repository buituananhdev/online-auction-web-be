using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Product : BaseDomainEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public ConditionEnum Condition { get; set; }
        public Int64 StartingPrice { get; set; }
        public Boolean canReturn { get; set; }
        public ProductStatusEnum productStatus { get; set; }
        public Int32 viewCount { get; set; }
        public int sellerId { get; set; }
        public User Seller { get; set; }
    }
}
