using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IWatchListService
    {
        Task<List<string>> GetListAuctionIdsByUserIDAsync(int userID);
        Task<List<string>> GetListUserIdsByAuctionIDAsync(int auctionID);
        Task AddToWatchListAsync(CreateWatchListDto createWatchListDto);
    }
    public class WatchListService : IWatchListService
    {
        private readonly DataContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public WatchListService(DataContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task AddToWatchListAsync(CreateWatchListDto createWatchListDto)
        {
            try
            {
                if(_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                createWatchListDto.UserId = (int)_currentUserService.UserId;
                bool isExist = _context.WatchList.Any(x => x.UserId == createWatchListDto.UserId && x.AuctionId == createWatchListDto.AuctionId);
                if(!isExist)
                {
                    _context.WatchList.Add(_mapper.Map<WatchList>(createWatchListDto));
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
                .Where(x => x.UserId == userID)
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
