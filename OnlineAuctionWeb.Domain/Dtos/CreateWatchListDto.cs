using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateWatchListDto
    {
        public CreateWatchListDto(int userId, int auctionId, WatchListTypeEnum type)
        {
            UserId = userId;
            AuctionId = auctionId;
            Type = type;
        }

        public int UserId { get; set; }
        public int AuctionId { get; set; }
        public WatchListTypeEnum Type { get; set; }

    }
}
