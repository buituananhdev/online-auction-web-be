﻿namespace OnlineAuctionWeb.Application.Hubs.Schemas
{
    public class HubUser
    {
        public int? UserId { get; set; }

        public HashSet<string> ConnectionIds { get; set; } = new();
    }
}
