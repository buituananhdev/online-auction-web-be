﻿using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class BidDto
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public int UserId { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
    }
}
