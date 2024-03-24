namespace OnlineAuctionWeb.Domain.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
        public Payment Payment { get; set; }
    }
}
