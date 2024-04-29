using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IBidService
    {
        Task<PaginatedResult<BidDto>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(CreateBidDto bidDto);
    }

    public class BidService : IBidService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAuctionService _auctionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ProductStatusEnum[] INVALID_AUCTION_STATUSES = new[]
        {
            ProductStatusEnum.Canceled, ProductStatusEnum.Ended, ProductStatusEnum.PendingPublish
        };

        public BidService(DataContext context, IMapper mapper, IAuctionService auctionService, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _auctionService = auctionService;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task CreateAsync(CreateBidDto bidDto)
        {
            try
            {
                if(_currentUserService.UserId == null)
                {
                      throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var auction = await _auctionService.GetByIdAsync(bidDto.AuctionId);
                if (bidDto.BidAmount <= auction.CurrentPrice || bidDto.BidAmount > auction.MaxPrice)
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Invalid bid amount!");
                }

                var currentTimestamp = DateTime.Now;
                if (currentTimestamp > auction.EndTime || INVALID_AUCTION_STATUSES.Contains(auction.ProductStatus))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Auction is not available for bidding!");
                }

                if (bidDto.BidAmount == auction.MaxPrice)
                {
                    await _auctionService.UpdateProductStatusAsync(auction.Id, ProductStatusEnum.Ended);
                }

                var bid = _mapper.Map<Bid>(bidDto);
                bid.UserId = (int)_currentUserService.UserId;

                await _context.Bids.AddAsync(bid);
                await _auctionService.UpdateCurrentPriceAsync(bidDto.AuctionId, bidDto.BidAmount);
                await _context.SaveChangesAsync();
                //_notificationService.SendNotificationAsync(auctionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw;
            }
        }

        public async Task<PaginatedResult<BidDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalBids = await _context.Bids.CountAsync();

                var bids = await _context.Bids
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalBids / pageSize);
                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };
                var result = new PaginatedResult<BidDto>
                {
                    Meta = meta,
                    Data = _mapper.Map<List<BidDto>>(bids)
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw;
            }
        }
    }
}
