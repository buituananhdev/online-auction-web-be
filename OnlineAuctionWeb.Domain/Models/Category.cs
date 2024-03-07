using OnlineAuctionWeb.Domain.Common;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Category : BaseDomainEntity
    {
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
