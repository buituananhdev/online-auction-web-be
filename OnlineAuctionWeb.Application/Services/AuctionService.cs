using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        Task<AuctionDto> GetDetailsAsync(int id);
        Task<AuctionDto> CreateAsync(CreateAuctionDto productDto);
        Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto);
        Task<AuctionDto> DeleteAsync(int id);
        Task ChangeStatusAsync(int id, ProductStatusEnum status);
        Task UpdateCurrentPriceAsync(int id, decimal price);
        Task UpdateProductStatusAsync(int id, ProductStatusEnum status);
        Task SeedData(int count);
        Task<List<AuctionDto>> GetListRecentlyViewedAsync();
        Task<List<AuctionDto>> GetTop10Auctions();
        Task<PaginatedResult<AuctionDto>> GetSellerAuctionsHistory(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ProductStatusEnum? status = null
        );
        Task<PaginatedResult<AuctionDto>> GetBuyerAuctionsHistory(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            bool isSuccess = false
        );
    }
    public class AuctionService : IAuctionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWatchListService _watchListService;
        private readonly IAuctionMediaService _auctionMediaService;

        public AuctionService(DataContext context, IMapper mapper, IFeedbackService feedbackService, ICurrentUserService currentUserService, IWatchListService watchListService, IAuctionMediaService auctionMediaService)
        {
            _context = context;
            _mapper = mapper;
            _feedbackService = feedbackService;
            _currentUserService = currentUserService;
            _watchListService = watchListService;
            _auctionMediaService = auctionMediaService;
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

                if (auction.UserId != _currentUserService.UserId || _currentUserService.Role != "Admin")
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "You are not authorized to change the status of this auction!");
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
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var auction = _mapper.Map<Auction>(auctionDto);
                auction.UserId = (int)_currentUserService.UserId;
                auction.CurrentPrice = auction.StartingPrice;
                _context.Auctions.Add(auction);
                await _auctionMediaService.AddMediasToAuction(auction.Id, auctionDto.mediasUrl);
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
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<PaginatedResult<AuctionDto>> GetBuyerAuctionsHistory(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            bool isSuccess = false
        )
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var userId = _currentUserService.UserId.Value;

                var query = _context.Auctions
                    .Include(a => a.Bids) // Include bids related to auction
                    .Include(a => a.User) // Include seller information
                    .Include(a => a.Category) // Include category information
                    .Where(a => _context.Bids.Any(b => b.UserId == userId && b.AuctionId == a.Id))
                    .AsQueryable();

                if (isSuccess)
                {
                    var winningAuctionIds = await _context.Bids
                        .Where(b => b.UserId == userId)
                        .GroupBy(b => b.AuctionId)
                        .Select(g => g.OrderByDescending(b => b.BidAmount).FirstOrDefault().AuctionId)
                        .Where(auctionId => _context.Auctions.Any(a => a.Id == auctionId && a.EndTime < DateTime.Now))
                        .ToListAsync();

                            query = query.Where(a => winningAuctionIds.Contains(a.Id));
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        a.ProductName.Contains(searchQuery) ||
                        a.Description.Contains(searchQuery)
                    );
                }

                var totalAuctions = await query.CountAsync();

                var auctions = await query
                    .OrderByDescending(a => a.EndTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);

                var auctionDtos = auctions.Select(auction =>
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
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
                Console.WriteLine("Err: " + ex.Message);
                throw;
            }
        }

        public async Task<AuctionDto> GetByIdAsync(int id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Bids)
                .FirstOrDefaultAsync(x => x.Id == id);

            var auctionDto = _mapper.Map<AuctionDto>(auction);
            auctionDto.BidCount = auction.Bids.Count;
            return auctionDto;
        }

        public async Task<AuctionDto> GetDetailsAsync(int id)
        {
            var auction = await _context.Auctions
                .Include(a => a.Bids)
                .Include(a => a.User)
                .AsQueryable()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
            }

            auction.ViewCount = auction.ViewCount++;
            await _context.SaveChangesAsync();
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            auctionDto.BidCount = auction.Bids.Count();
            auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);

            if (_currentUserService.UserId != null)
            {
                await _watchListService.AddToWatchListAsync(new CreateWatchListDto((int)_currentUserService.UserId, id, WatchListTypeEnum.RecentlyViewed));
            }

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

                var query = _context.WatchList
                    .Where(wl => wl.UserId == _currentUserService.UserId && wl.Type == WatchListTypeEnum.RecentlyViewed)
                    .Join(_context.Auctions, wl => wl.AuctionId, a => a.Id, (wl, a) => a)
                    .Include(a => a.Bids)
                    .Include(a => a.Category)
                    .Include(a => a.User)
                    .AsQueryable();

                var auctions = await query.ToListAsync();

                var auctionDtos = new List<AuctionDto>();
                foreach (var auction in auctions)
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDtos.Add(auctionDto);
                }

                return auctionDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<PaginatedResult<AuctionDto>> GetSellerAuctionsHistory(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ProductStatusEnum? status = null
        )
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                int userId = (int)_currentUserService.UserId;

                var query = _context.Auctions
                    .Include(a => a.Bids) // Include bids related to auction
                    .Include(a => a.User) // Include seller information
                    .Include(a => a.Category) // Include category information
                    .Where(a => a.UserId == userId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        a.ProductName.Contains(searchQuery) ||
                        a.Description.Contains(searchQuery)
                    );
                }

                if (status != null)
                {
                    query = query.Where(a => a.ProductStatus == status);
                }

                var totalAuctions = await query.CountAsync();

                var auctions = await query
                    .OrderByDescending(a => a.EndTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);

                var auctionDtos = auctions.Select(auction =>
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
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
                Console.WriteLine("Err: " + ex.Message);
                throw;
            }
        }

        public async Task<List<AuctionDto>> GetTop10Auctions()
        {
            try
            {
                var topAuctions = await _context.Auctions
                    .Include(a => a.Bids)
                    .Include(a => a.User)
                    .Include(a => a.Category)
                    .Where(a => a.EndTime > DateTime.Now && a.Bids.Any())
                    .OrderByDescending(a => a.Bids.Count)
                    .Take(10)
                    .ToListAsync();

                var auctionDtos = new List<AuctionDto>();
                foreach (var auction in topAuctions)
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDtos.Add(auctionDto);
                }

                return auctionDtos;
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
