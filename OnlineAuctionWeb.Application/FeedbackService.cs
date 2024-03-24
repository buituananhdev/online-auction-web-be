using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Application
{
    public interface IFeedbackService
    {
        Task<AvarageRatingPayload> GetAverageRatingByUserIdAsync(int userId);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly DataContext _context;
        public FeedbackService(DataContext context)
        {
            _context = context;
        }
        public async Task<AvarageRatingPayload> GetAverageRatingByUserIdAsync(int userId)
        {
            var averageRating = await _context.Feedbacks
                .Where(f => f.ToUserId == userId)
                .Select(f => f.Rating)
                .DefaultIfEmpty()
                .AverageAsync();
            var payload = new AvarageRatingPayload
            {
                AvarageRating = averageRating,
                TotalRatings = await _context.Feedbacks.CountAsync(f => f.ToUserId == userId)
            };
            return payload;
        }
    }
}
