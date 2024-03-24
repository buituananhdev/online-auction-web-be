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
        Task CreateAsync(CreateUserDto userDto);
        Task<UserDto> UpdateAsync(int id, UserDto userDto);
        Task DeleteAsync(int id);
        Task<UserDto> FindUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<int> GetUserRoleByIdAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;
        public UserService(DataContext contex, IMapper mapper, IFeedbackService feedbackService)
        {
            _context = contex;
            _mapper = mapper;
            _feedbackService = feedbackService;
        }
        public async Task CreateAsync(CreateUserDto userDto)
        {
            try
            {
                if (await UserExistsByEmailAsync(userDto.Email))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Email already exists!");
                }

                User user = _mapper.Map<User>(userDto);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashedPassword;
                user.Status = StatusEnum.Active;
                if(string.IsNullOrEmpty(user.FullName))
                {
                    user.FullName = "User@" + Guid.NewGuid().ToString().Substring(0, 6);
                }
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw new CustomException(500, ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
            }

            user.Status = StatusEnum.Inactive;
            await _context.SaveChangesAsync();
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

                var userDtos = _mapper.Map<List<UserDto>>(users);

                userDtos.ForEach(async user =>
                {
                    user.ratings = await _feedbackService.GetAverageRatingByUserIdAsync(user.Id);
                });

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
                    Data = userDtos
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
                var userDto = _mapper.Map<UserDto>(user);
                userDto.ratings = await _feedbackService.GetAverageRatingByUserIdAsync(id);
                return userDto;
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
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public Task<UserDto> UpdateAsync(int id, UserDto userDto)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetUserRoleByIdAsync(int id)
        {
            var role = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => u.Role)
            .FirstOrDefaultAsync();

            return role;
        }
    }
}
