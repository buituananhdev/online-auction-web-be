﻿using AutoMapper;
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
            int? id,
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ProductStatusEnum? status = null
        );
        Task<PaginatedResult<AuctionDto>> GetBuyerAuctionsHistory(
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            BidStatusEnum? status = null
        );

        Task UpdatePredictAvgPrice(int auctionId, decimal predictAvgPrice);
        Task<SellerRevenuePayload> GetSellerRevenue();
    }
    public class AuctionService : IAuctionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWatchListService _watchListService;
        private readonly IAuctionMediaService _auctionMediaService;
        private readonly IHubService _hubService;
        private readonly IUserService _userService;
        private readonly ProductStatusEnum[] INVALID_AUCTION_STATUSES = new[]
        {
            ProductStatusEnum.Canceled, ProductStatusEnum.Ended, ProductStatusEnum.PendingPublish
        };

        public AuctionService(DataContext context, IMapper mapper, IFeedbackService feedbackService, ICurrentUserService currentUserService, IWatchListService watchListService, IAuctionMediaService auctionMediaService, IHubService hubService, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _feedbackService = feedbackService;
            _currentUserService = currentUserService;
            _watchListService = watchListService;
            _auctionMediaService = auctionMediaService;
            _hubService = hubService;
            _userService = userService;
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

                if (auction.UserId != _currentUserService.UserId && _currentUserService.Role != "Admin")
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "You are not authorized to change the status of this auction!");
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
                await _context.SaveChangesAsync();
                await _auctionMediaService.AddMediasToAuction(auction.Id, auctionDto.mediaUrls);
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
                    .Where(a => !INVALID_AUCTION_STATUSES.Contains(a.ProductStatus))
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
                    auctionDto.IsWatched = _watchListService.IsWatched(auction.Id);
                    auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);
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
            BidStatusEnum? status = null
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

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        a.ProductName.Contains(searchQuery) ||
                        a.Description.Contains(searchQuery)
                    );
                }
                if(status != null)
                {
                    if (status == BidStatusEnum.Win)
                    {
                        var winningAuctionIds = await _context.Bids
                            .Where(b => b.UserId == userId)
                            .GroupBy(b => b.AuctionId)
                            .Select(g => g.OrderByDescending(b => b.BidAmount).FirstOrDefault().AuctionId)
                            .Where(auctionId => _context.Auctions.Any(a => a.Id == auctionId && a.EndTime < DateTime.Now))
                            .ToListAsync();

                        query = query.Where(a => winningAuctionIds.Contains(a.Id));
                    }
                    else if (status == BidStatusEnum.InProgress)
                    {
                        query = query.Where(a => a.EndTime > DateTime.Now && a.ProductStatus == ProductStatusEnum.InProgess);
                    }
                    else if (status == BidStatusEnum.Lose)
                    {
                        var winningAuctionIds = await _context.Bids
                            .Where(b => b.UserId == userId)
                            .GroupBy(b => b.AuctionId)
                            .Select(g => g.OrderByDescending(b => b.BidAmount).FirstOrDefault().AuctionId)
                            .Where(auctionId => _context.Auctions.Any(a => a.Id == auctionId && a.EndTime < DateTime.Now))
                            .ToListAsync();

                        query = query.Where(a => !winningAuctionIds.Contains(a.Id));
                        query = query.Where(a => a.EndTime < DateTime.Now && (a.ProductStatus == ProductStatusEnum.Ended || a.ProductStatus == ProductStatusEnum.Canceled));
                    }
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
                    auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);
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
                .Include(x => x.User)
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

            auction.ViewCount++;
            await _context.SaveChangesAsync();
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            auctionDto.BidCount = auction.Bids.Count();
            auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
            auctionDto.IsWatched = _watchListService.IsWatched(auction.Id);
            auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);

            if (_currentUserService.UserId != null)
            {
                var user = await _userService.GetByIdAsync((int)_currentUserService.UserId);
                if(user.Role == RoleEnum.Buyer)
                {
                    await _watchListService.AddToWatchListAsync(new CreateWatchListDto(id, WatchListTypeEnum.RecentlyViewed));
                    await _hubService.AddUserToGroupHub(auction.Id, (int)_currentUserService.UserId);
                }
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
                    .Join(_context.Auctions.Where(a => !INVALID_AUCTION_STATUSES.Contains(a.ProductStatus)),
                        wl => wl.AuctionId,
                        a => a.Id,
                        (wl, a) => a)
                    .Include(a => a.Bids)
                    .Include(a => a.Category)
                    .Include(a => a.User)
                    .AsQueryable();


                var auctions = await query.ToListAsync();

                var auctionDtos = new List<AuctionDto>();
                foreach (var auction in auctions)
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDto.IsWatched = _watchListService.IsWatched(auction.Id);
                    auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);
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
            int? id,
            int pageNumber,
            int pageSize,
            string searchQuery = null,
            ProductStatusEnum? status = null
        )
        {
            try
            {
                int userId = id ?? (int)_currentUserService.UserId;

                if (userId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var query = _context.Auctions
                    .Include(a => a.User) // Include seller information
                    .Include(a => a.Category) // Include category information
                    .Include(a => a.Bids)
                    .ThenInclude(b => b.User)
                    .Include(a => a.Bids)
                    .ThenInclude(b => b.Payment)
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
                    switch (status)
                    {
                        case ProductStatusEnum.Ended:
                            query = query.Where(a => a.EndTime < DateTime.Now || a.ProductStatus == ProductStatusEnum.Canceled || a.ProductStatus == ProductStatusEnum.Ended);
                            break;
                        case ProductStatusEnum.InProgess:
                            query = query.Where(a => a.EndTime > DateTime.Now && a.ProductStatus != ProductStatusEnum.Canceled && a.ProductStatus != ProductStatusEnum.Ended);
                            break;
                        case ProductStatusEnum.Canceled:
                            query = query.Where(a => a.ProductStatus == ProductStatusEnum.Canceled);
                            break;
                    }
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
                    auctionDto.User = null;

                    foreach (var bid in auction.Bids)
                    {
                        if (bid.Payment != null)
                        {
                            auctionDto.User = _mapper.Map<UserDto>(bid.User);
                            break;
                        }
                    }
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);
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

        public async Task<SellerRevenuePayload> GetSellerRevenue()
        {
            try
            {
                if(_currentUserService.Role != RoleEnum.Seller.ToString())
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "You are not authorized to view this information!");
                }

                var auctions = await _context.Auctions
                    .Include(a => a.Bids)
                    .ThenInclude(b => b.Payment)
                    .Where(a => a.UserId == _currentUserService.UserId).ToListAsync();
                var totalAuctions = auctions.Count;
                var soldAuctions = auctions.Where(a => a.Bids.Any(b => b.Payment != null)).ToList();
                var totalSoldAuctions = soldAuctions.Count;

                var totalRevenue = soldAuctions
                           .SelectMany(a => a.Bids)
                           .Where(b => b.Payment != null)
                           .Sum(b => b.Payment.Amount ?? 0);

                var sellerRevenuePayload = new SellerRevenuePayload
                {
                    TotalAuctions = totalAuctions,
                    TotalSoldAuctions = totalSoldAuctions,
                    TotalRevenue = totalRevenue
                };

                return sellerRevenuePayload;
            }
            catch
            {
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
                    .Where(a => a.EndTime > DateTime.Now && a.Bids.Any() && !INVALID_AUCTION_STATUSES.Contains(a.ProductStatus))
                    .OrderByDescending(a => a.Bids.Count)
                    .Take(10)
                    .ToListAsync();

                var auctionDtos = new List<AuctionDto>();
                foreach (var auction in topAuctions)
                {
                    var auctionDto = _mapper.Map<AuctionDto>(auction);
                    auctionDto.BidCount = auction.Bids.Count();
                    auctionDto.ViewCount = auction.ViewCount;
                    auctionDto.User = _mapper.Map<UserDto>(auction.User);
                    auctionDto.User.ratings = _feedbackService.GetAverageRatingByUserId(auction.UserId);
                    auctionDto.Category = _mapper.Map<CategoryDto>(auction.Category);
                    auctionDto.IsWatched = _watchListService.IsWatched(auction.Id);
                    auctionDto.mediaUrls = _auctionMediaService.GetAuctionMediaUrls(auction.Id);
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

        public async Task UpdatePredictAvgPrice(int auctionId, decimal predictAvgPrice)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(auctionId);
                if(auction == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Auction not found!");
                }

                auction.PredictAvgPrice = predictAvgPrice;
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
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
