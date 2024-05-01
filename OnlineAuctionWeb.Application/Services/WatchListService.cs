using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IWatchListService
    {
        Task<List<string>> GetListAuctionIdsByUserIDAsync(int userID);
        Task<List<string>> GetListUserIdsByAuctionIDAsync(int auctionID);
        Task AddToWatchListAsync(CreateWatchListDto createWatchListDto);
        Task<PaginatedResult<AuctionDto>> GetWatchListByUserIDAsync(
            int pageNumber,
            int pageSize,
            string? searchQuery = null,
            WatchListTypeEnum? type = null
       );
        bool IsWatched(int auctionID);
    }
    public class WatchListService : IWatchListService
    {
        private readonly DataContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;

        public WatchListService(DataContext context, ICurrentUserService currentUserService, IMapper mapper, IFeedbackService feedbackService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _feedbackService = feedbackService;
        }

        public async Task AddToWatchListAsync(CreateWatchListDto createWatchListDto)
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var watchlistDto = _mapper.Map<WatchList>(createWatchListDto);
                watchlistDto.UserId = (int)_currentUserService.UserId;
                watchlistDto.Type = WatchListTypeEnum.WatchList;
                bool isExist = _context.WatchList.Any(x => x.UserId == watchlistDto.UserId && x.AuctionId == watchlistDto.AuctionId);
                if (!isExist)
                {
                    _context.WatchList.Add(watchlistDto);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<string>> GetListAuctionIdsByUserIDAsync(int userID)
        {
            try
            {
                var watchLists = await _context.WatchList
                .Where(x => x.UserId == userID && x.Type == WatchListTypeEnum.WatchList)
                .Select(x => x.AuctionId.ToString())
                .ToListAsync();

                return watchLists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<string>> GetListUserIdsByAuctionIDAsync(int auctionID)
        {
            try
            {
                var userIds = await _context.WatchList
                .Where(x => x.AuctionId == auctionID)
                .Select(x => x.UserId.ToString())
                .ToListAsync();

                return userIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<PaginatedResult<AuctionDto>> GetWatchListByUserIDAsync(
            int pageNumber,
            int pageSize,
            string? searchQuery = null,
            WatchListTypeEnum? type = null
        )
        {
            try
            {
                var query = _context.WatchList
                    .Include(w => w.Auction)
                        .ThenInclude(a => a.Category)
                    .Include(w => w.Auction)
                        .ThenInclude(a => a.Bids)
                    .Include(w => w.Auction)
                        .ThenInclude(a => a.User)
                    .Where(w => w.UserId == _currentUserService.UserId);

                if (type != null)
                {
                    query = query.Where(w => w.Type == type);
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        EF.Functions.Like(a.Auction.ProductName, $"%{searchQuery}%") ||
                        EF.Functions.Like(a.Auction.Description, $"%{searchQuery}%"));
                }

                var totalAuctions = await query.CountAsync();

                var auctions = await query
                    .Select(w => w.Auction)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);

                // Map auctions to AuctionDtos
                var auctionDtos = new List<AuctionDto>();
                foreach (var auction in auctions)
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDtos.Add(auctionDto);
                }

                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };

                var result = new PaginatedResult<AuctionDto>
                {
                    Meta = meta,
                    Data = auctionDtos
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public bool IsWatched(int auctionID)
        {
            try 
            {
                return _context.WatchList.Any(x => x.UserId == _currentUserService.UserId && x.AuctionId == auctionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
