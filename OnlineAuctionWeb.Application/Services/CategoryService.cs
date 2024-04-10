using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Application.Exceptions;

namespace OnlineAuctionWeb.Application.Services
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategoryDto categoryDto);
        Task<PaginatedResult<CategoryDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto productDto);
        Task<CategoryDto> DeleteAsync(int id);
        Task ChangeStatusAsync(int id, StatusEnum status);

    }
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CategoryService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task ChangeStatusAsync(int id, StatusEnum status)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category is null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Category not found!");
                }

                category.Status = status;
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task CreateAsync(CreateCategoryDto categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw new Exception(ex.Message);
            }
        }

        public Task<CategoryDto> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<CategoryDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalCaregories = await _context.Categories.CountAsync();

                var categories = await _context.Categories
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCaregories / pageSize);
                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };
                var result = new PaginatedResult<CategoryDto>
                {
                    Meta = meta,
                    Data = _mapper.Map<List<CategoryDto>>(categories)
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public Task<CategoryDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category is null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Category not found!");
                }

                _mapper.Map(categoryDto, category);
                await _context.SaveChangesAsync();
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }
    }
}
