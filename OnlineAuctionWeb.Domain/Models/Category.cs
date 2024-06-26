﻿using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Domain.Models
{
    public class Category : BaseDomainEntity
    {
        public string CategoryName { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public string? ImageUrl { get; set; }
        public List<Auction> Products { get; set; }
    }
}
