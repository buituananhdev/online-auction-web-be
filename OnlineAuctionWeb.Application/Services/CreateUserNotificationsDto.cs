using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Application.Services
{
    public class CreateUserNotificationsDto
    {

        public int UserId { get; set; }
        public int NotificationId { get; set; }
        public Boolean IsRead { get; set; } = false;
    }
}
