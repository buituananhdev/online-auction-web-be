using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BidId { get; set; }
        public Bid Bid { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
