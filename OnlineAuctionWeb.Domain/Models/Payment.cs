using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Payment : BaseDomainEntity
    {
        public int BidId { get; set; }
        public Bid Bid { get; set; }
        public VnpayResponseCode ResponseCode { get; set; }
        public string? TransactionNumber { get; set; }
        public string? Bank { get; set; }
        public decimal? Amount { get; set; }
    }
}
