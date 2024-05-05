using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Notification : BaseDomainEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? RedirectUrl { get; set; }
        public int? RelatedID { get; set; }
        public NotificationType Type { get; set; }
        public List<UserNotification> UserNotifications { get; set; }
    }
}
