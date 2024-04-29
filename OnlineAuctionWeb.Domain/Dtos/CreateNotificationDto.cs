using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateNotificationDto
    {

        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? RedirectUrl { get; set; }
        public int? RelatedID { get; set; }
        public NotificationType Type { get; set; }
    }
}
