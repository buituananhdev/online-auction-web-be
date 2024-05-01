using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IAuctionMediaService
    {
        Task AddMediasToAuction(int auctionId, List<string> mediasUrl);
    }

    public class AuctionMediaService : IAuctionMediaService
    {
        private readonly DataContext _context;
        public AuctionMediaService(DataContext context)
        {
            _context = context;   
        }
        public async Task AddMediasToAuction(int auctionId, List<string> mediasUrl)
        {
            try
            {
                foreach(var mediaUrl in mediasUrl)
                {
                    _context.AuctionMedias.Add(new AuctionMedia
                    {
                        AuctionId = auctionId,
                        MediaUrl = mediaUrl
                    });
                }
                await _context.SaveChangesAsync();
            } catch(Exception ex)
            {
                throw;
            }
        }
    }
}
