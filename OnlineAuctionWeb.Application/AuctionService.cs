using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Infrastructure.Exceptions;
using OnlineAuctionWeb.Infrastructure.Utils;

namespace OnlineAuctionWeb.Application
{
    public interface IAuctionService
    {
        Task<PaginatedResult<AuctionDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ConditionEnum? condition = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null);
        Task<AuctionDto> GetByIdAsync(int id);
        Task CreateAsync(CreateAuctionDto productDto, int userId);
        Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto);
        Task<AuctionDto> DeleteAsync(int id);
    }
    public class AuctionService : IAuctionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public AuctionService(DataContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task CreateAsync(CreateAuctionDto auctionDto, int userId)
        {
            try
            {
                var auction = _mapper.Map<Auction>(auctionDto);
                auction.SellerId = userId;
                auction.CurrentPrice = auction.StartingPrice;
                _context.Auctions.Add(auction);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<AuctionDto> DeleteAsync(int id)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(id);
                if (auction == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
                }
                auction.ProductStatus = ProductStatusEnum.Canceled;
                await _context.SaveChangesAsync();
                return _mapper.Map<AuctionDto>(auction);
            } catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public Task<AuctionDto> FindProductByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<AuctionDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ConditionEnum? condition = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null)
        {
            try
            {
                var query = _context.Auctions
                    .Include(a => a.Bids) // Include bids related to auction
                    .Include(a => a.Seller) // Include seller information
                    .AsQueryable();

                // Filter by search query (fuzzy search)
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        EF.Functions.Like(a.ProductName, $"%{searchQuery}%") ||
                        EF.Functions.Like(a.Description, $"%{searchQuery}%"));
                }

                // Filter by condition
                if (condition != null)
                {
                    query = query.Where(a => a.Condition == condition);
                }

                // Filter by starting price range
                if (minCurrentPrice != null)
                {
                    query = query.Where(a => a.CurrentPrice >= minCurrentPrice);
                }
                if (maxCurrentPrice != null)
                {
                    query = query.Where(a => a.CurrentPrice <= maxCurrentPrice);
                }

                // Filter by max price range
                if (minMaxPrice != null)
                {
                    query = query.Where(a => a.MaxPrice >= minMaxPrice);
                }
                if (maxMaxPrice != null)
                {
                    query = query.Where(a => a.MaxPrice <= maxMaxPrice);
                }

                // Filter by end time range
                if (minEndTime != null)
                {
                    query = query.Where(a => a.EndTime >= minEndTime);
                }
                if (maxEndTime != null)
                {
                    query = query.Where(a => a.EndTime <= maxEndTime);
                }

                var totalAuctions = await query.CountAsync();

                var auctions = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);

                // Map auctions to AuctionDtos
                var auctionDtos = auctions.Select(auction =>
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.Seller = _mapper.Map<UserDto>(auction.Seller);

                    return auctionDto;
                }).ToList();

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
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<AuctionDto> GetByIdAsync(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
            }
            return _mapper.Map<AuctionDto>(auction);
        }

        public Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto)
        {
            throw new NotImplementedException();
        }
    }
}
