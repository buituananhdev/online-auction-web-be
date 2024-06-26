﻿using OnlineAuctionWeb.Domain.Common;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Feedback : BaseDomainEntity
    {
        public int FromUserId { get; set; }
        public User FromUser { get; set; }
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public int Rating { get; set; }
        public string? FeedbackMessage { get; set; }
        public int RelatedID { get; set; }
    }
}
