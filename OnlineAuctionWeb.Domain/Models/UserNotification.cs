﻿namespace OnlineAuctionWeb.Domain.Models
{
    public class UserNotification
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public Boolean IsRead { get; set; }
    }
}