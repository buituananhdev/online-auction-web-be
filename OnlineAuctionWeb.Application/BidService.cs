using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Application
{
    public interface IBidService
    {
        Task<PaginatedResult<BidDto>> GetAllAsync(int pageNumber, int pageSize);
    }

    public class BidService : IBidService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public BidService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
