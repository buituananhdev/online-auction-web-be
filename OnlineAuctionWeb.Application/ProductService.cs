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
    public interface IProductService
    {
        Task<PaginatedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductDto> GetByIdAsync(int id);
        Task CreateAsync(CreateProductDto productDto);
        Task<ProductDto> UpdateAsync(int id, ProductDto productDto);
        Task<ProductDto> DeleteAsync(int id);
    }
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ProductService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public Task<ProductDto> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> FindProductByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync();

                var products = await _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };
                var result = new PaginatedResult<ProductDto>
                {
                    Meta = meta,
                    Data = _mapper.Map<List<ProductDto>>(products)
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, "Product not found!");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public Task<ProductDto> UpdateAsync(int id, ProductDto productDto)
        {
            throw new NotImplementedException();
        }
    }
}
