using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Infrastructure.Exceptions;

namespace OnlineAuctionWeb.Application
{
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<UserDto> GetByIdAsync(int id);
        Task<Boolean> CreateAsync(UserDto userDto);
        Task<UserDto> UpdateAsync(int id, UserDto userDto);
        Task<UserDto> DeleteAsync(int id);
        Task<UserDto> FindUserByEmailAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserService(DataContext contex, IMapper mapper)
        {
            _context = contex;
            _mapper = mapper;
        }
        public async Task<Boolean> CreateAsync(UserDto userDto)
        {
            try
            {
                User user = _mapper.Map<User>(userDto);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashedPassword;
                user.IsActive = StatusEnum.Active;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                if (await FindUserByEmailAsync(user.Email) != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw new CustomException(500, ex.Message);
            }
        }

        public Task<UserDto> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<UserDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();

                var users = await _context.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };
                var result = new PaginatedResult<UserDto>
                {
                    Meta = meta,
                    Data = _mapper.Map<List<UserDto>>(users)
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<UserDto> FindUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public Task<UserDto> UpdateAsync(int id, UserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
