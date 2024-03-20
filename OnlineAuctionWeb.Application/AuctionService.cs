using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Infrastructure.Exceptions;

namespace OnlineAuctionWeb.Application
{
    public interface IAuctionService
    {
        Task<PaginatedResult<AuctionDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<AuctionDto> GetByIdAsync(int id);
        Task CreateAsync(CreateAuctionDto productDto);
        Task<AuctionDto> UpdateAsync(int id, AuctionDto productDto);
        Task<AuctionDto> DeleteAsync(int id);
    }
    public class AuctionService : IAuctionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public AuctionService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateAuctionDto productDto)
        {
            try
            {
                var auction = _mapper.Map<Auction>(productDto);
                _context.Auctions.Add(auction);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public Task<AuctionDto> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AuctionDto> FindProductByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<AuctionDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalAuctions = await _context.Auctions.CountAsync();

                var auctions = await _context.Auctions
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);
                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };
                var result = new PaginatedResult<AuctionDto>
                {
                    Meta = meta,
                    Data = _mapper.Map<List<AuctionDto>>(auctions)
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
