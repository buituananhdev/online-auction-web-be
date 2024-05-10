using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CategoryDto : BaseDomainEntity
    {
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public StatusEnum Status { get; set; }

    }
}
