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
    public class ProductDto : BaseDomainEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public ConditionEnum Condition { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean CanReturn { get; set; }
        public ProductStatusEnum ProductStatus { get; set; }
        public Int32 ViewCount { get; set; }
        public int SellerId { get; set; }
        public int CategoryId { get; set; }
    }
}
