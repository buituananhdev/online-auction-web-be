using OnlineAuctionWeb.Domain.Common;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class AuctionDto : BaseDomainEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public ConditionEnum Condition { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean CanReturn { get; set; }
        public int BidCount { get; set; }
        public ProductStatusEnum ProductStatus { get; set; }
        public Int32 ViewCount { get; set; }
        public UserDto User { get; set; }
        public string CategoryName { get; set; }
    }
}
