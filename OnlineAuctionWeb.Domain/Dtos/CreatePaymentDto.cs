using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreatePaymentDto
    {
        public int BidId { get; set; }
        public VnpayResponseCode ResponseCode { get; set; }
        public string? TransactionNumber { get; set; }
        public string? Bank { get; set; }
        public decimal? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
