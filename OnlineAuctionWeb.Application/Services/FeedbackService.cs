using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Payloads;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IFeedbackService
    {
        AvarageRatingPayload GetAverageRatingByUserId(int userId);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly DataContext context;
        public FeedbackService(DataContext context)
        {
            this.context = context;
        }
        public AvarageRatingPayload GetAverageRatingByUserId(int userId)
        {
            var averageRating = context.Feedbacks
                .Where(f => f.ToUserId == userId)
                .Select(f => f.Rating)
                .DefaultIfEmpty()
                .Average();
            var payload = new AvarageRatingPayload
            {
                AvarageRating = averageRating,
                TotalRatings = context.Feedbacks.Count(f => f.ToUserId == userId)
            };

            return payload;
        }
    }
}
