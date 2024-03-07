using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Models
{
    public class ProductMedia
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
