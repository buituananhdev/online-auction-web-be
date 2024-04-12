using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;
using System.Text.Json;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IAuctionService
    {
        Task<PaginatedResult<AuctionDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            string? conditions = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null,
            string? categoryIds = null);
        Task<AuctionDto> GetByIdAsync(int id);
        Task<AuctionDto> CreateAsync(CreateAuctionDto productDto);
        Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto);
        Task<AuctionDto> DeleteAsync(int id);
        Task ChangeStatusAsync(int id, ProductStatusEnum status);
        Task UpdateCurrentPriceAsync(int id, decimal price);
        Task UpdateProductStatusAsync(int id, ProductStatusEnum status);
        Task SeedData(int count);
        Task<List<AuctionDto>> GetListRecentlyViewedAsync();
        Task<List<AuctionDto>> GetTop10Auctions();
    }
    public class AuctionService : IAuctionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICurrentUserService _currentUserService;

        public AuctionService(DataContext context, IMapper mapper, IFeedbackService feedbackService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _feedbackService = feedbackService;
            _currentUserService = currentUserService;
        }

        public async Task ChangeStatusAsync(int id, ProductStatusEnum status)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(id);
                if (auction is null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
                }

                auction.ProductStatus = status;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<AuctionDto> CreateAsync(CreateAuctionDto auctionDto)
        {
            try
            {
                if(_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var auction = _mapper.Map<Auction>(auctionDto);
                auction.UserId = _currentUserService.UserId;
                auction.CurrentPrice = auction.StartingPrice;
                _context.Auctions.Add(auction);
                await _context.SaveChangesAsync();

                return _mapper.Map<AuctionDto>(auction); 
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
            }
            catch (Exception ex)
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
            string? conditions = null,
            decimal? minCurrentPrice = null,
            decimal? maxCurrentPrice = null,
            decimal? minMaxPrice = null,
            decimal? maxMaxPrice = null,
            DateTime? minEndTime = null,
            DateTime? maxEndTime = null,
            string? categoryIds = null)
        {
            try
            {
                var query = _context.Auctions
                    .Include(a => a.Bids) // Include bids related to auction
                    .Include(a => a.User) // Include seller information
                    .Include(a => a.Category) // Include category information
                    .AsQueryable();

                // Filter by search query (fuzzy search)
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        EF.Functions.Like(a.ProductName, $"%{searchQuery}%") ||
                        EF.Functions.Like(a.Description, $"%{searchQuery}%"));
                }

                // Filter by condition
                if (conditions != null)
                {
                    List<int> conditionInt = JsonSerializer.Deserialize<List<int>>(conditions);
                    query = query.Where(a => conditionInt.Contains((int)a.Condition));
                }

                // Filter by current price range
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

                // Filter by category
                if (categoryIds != null)
                {
                    List<int> categoryIdsInt = JsonSerializer.Deserialize<List<int>>(categoryIds);
                    query = query.Where(a => categoryIdsInt.Contains(a.CategoryId));
                }

                var totalAuctions = await query.CountAsync();

                var auctions = await query
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
                    auctionDto.CategoryName = auction.Category.CategoryName;
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
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<AuctionDto> GetByIdAsync(int id)
        {
            var auction = await _context.Auctions.Include(a => a.Bids).AsQueryable().FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
            }

            var auctionDto = _mapper.Map<AuctionDto>(auction);
            auctionDto.BidCount = auction.Bids.Count();
            return auctionDto;
        }

        public async Task<List<AuctionDto>> GetListRecentlyViewedAsync()
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var auctions = await _context.WatchList
                .Where(wl => wl.UserId == _currentUserService.UserId && wl.Type == WatchListTypeEnum.RecentlyViewed)
                .Include(a => a.Auction)
                .Select(wl => wl.Auction)
                .ToListAsync();

                return _mapper.Map<List<AuctionDto>>(auctions);
            } catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<List<AuctionDto>> GetTop10Auctions()
        {
            try
            {
                var topAuctions = await _context.Auctions
                .Where(a => a.EndTime > DateTime.Now && a.Bids.Any())
                .OrderByDescending(a => a.Bids.Count)
                .Take(10)
                .Select(a => new AuctionDto
                {
                    Id = a.Id,
                    ProductName = a.ProductName,
                    Description = a.Description,
                    Condition = a.Condition,
                    StartingPrice = a.StartingPrice,
                    MaxPrice = a.MaxPrice,
                    CurrentPrice = a.CurrentPrice,
                    EndTime = a.EndTime,
                    BidCount = a.Bids.Count,
                    ProductStatus = a.ProductStatus,
                    ViewCount = a.ViewCount,
                    User = _mapper.Map<UserDto>(a.User),
                    CategoryName = a.Category.CategoryName
                })
                .ToListAsync();

                return topAuctions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task SeedData(int count)
        {
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                var product = new Auction
                {
                    ProductName = $"Iphone {i + 1} promax vip {random.Next(0, 512)}GB",
                    Description = $"Description for Product {i + 1}",
                    Condition = (ConditionEnum)random.Next(0, 3), // Assume ConditionEnum has 3 values
                    StartingPrice = random.Next(10, 1000),
                    MaxPrice = random.Next(1000, 5000),
                    CurrentPrice = random.Next(10, 1000),
                    EndTime = DateTime.Now.AddDays(random.Next(1, 30)),
                    CanReturn = random.Next(0, 2) == 1,
                    ProductStatus = (ProductStatusEnum)random.Next(0, 3), // Assume ProductStatusEnum has 3 values
                    ViewCount = random.Next(0, 1000),
                    UserId = 17, // SellerId is 22 as specified
                    CategoryId = random.Next(3, 4) // Assuming you have 10 categories
                };

                await _context.AddAsync(product);
            }

            await _context.SaveChangesAsync();
        }

        public Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCurrentPriceAsync(int id, decimal price)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(id);

                auction.CurrentPrice = price;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task UpdateProductStatusAsync(int id, ProductStatusEnum status)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(id);

                auction.ProductStatus = status;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }
    }
}
