using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IFeedbackService
    {
        AvarageRatingPayload GetAverageRatingByUserId(int userId);
        Task CreateAsync(CreateFeedBackDto createFeedBackDto);
        Task<List<FeedBackDto>> GetFeedBackByUserId(int userId);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public FeedbackService(DataContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task CreateAsync(CreateFeedBackDto createFeedBackDto)
        {
            try
            {
                var isExist = _context.Feedbacks.Any(f => f.FromUserId == _currentUserService.UserId && f.ToUserId == createFeedBackDto.ToUserId && f.RelatedID == createFeedBackDto.RelatedID);
                if (isExist)
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "You have already rated this user!");
                }
                var feedbackDto = _mapper.Map<Feedback>(createFeedBackDto);
                feedbackDto.FromUserId = (int)_currentUserService.UserId;
                _context.Feedbacks.Add(feedbackDto);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                throw;
            }
        }
        public AvarageRatingPayload GetAverageRatingByUserId(int userId)
        {
            var averageRating = _context.Feedbacks
                .Where(f => f.ToUserId == userId)
                .Select(f => f.Rating)
                .DefaultIfEmpty()
                .Average();
            var payload = new AvarageRatingPayload
            {
                AvarageRating = averageRating,
                TotalRatings = _context.Feedbacks.Count(f => f.ToUserId == userId)
            };

            return payload;
        }

        public async Task<List<FeedBackDto>> GetFeedBackByUserId(int userId)
        {
            try {
                var feedbacks = await _context.Feedbacks
                    .Where(f => f.ToUserId == userId)
                    .ToListAsync();

                return _mapper.Map<List<FeedBackDto>>(feedbacks);
            } catch(Exception ex) {
                throw;
            }
        }
    }
}
