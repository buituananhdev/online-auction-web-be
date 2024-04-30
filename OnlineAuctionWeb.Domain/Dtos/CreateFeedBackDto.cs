using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Domain.Dtos
{
    public class CreateFeedBackDto
    {
        public int ToUserId { get; set; }
        public int Rating { get; set; }
        public string? FeedbackMessage { get; set; }
        public int RelatedID { get; set; }
    }
}
