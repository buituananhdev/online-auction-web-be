using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class User : BaseDomainEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public int Role { get; set; }
        public StatusEnum Status { get; set; }
        public List<Auction> Products { get; set; }
        public List<Bid> Bids { get; set; }
        public List<Feedback> SentFeedbacks { get; set; }
        public List<Feedback> ReceivedFeedbacks { get; set; }
        public List<UserNotification> UserNotifications { get; set; }
    }
}
