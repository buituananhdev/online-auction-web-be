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
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetAllAsync(
            int pageNumber, 
            int pageSize, 
            string searchQuery = null,
            StatusEnum status = 0);
        Task<UserDto> GetByIdAsync(int id);
        Task CreateAsync(CreateUserDto userDto);
        Task<User> CreateGoogleUserAsync(CreateUserDto userDto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto userDto);
        Task DeleteAsync(int id);
        User FindUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<int> GetUserRoleByIdAsync(int id);
        Task<UserDto> GetMe();
        Task ChangePassword(ChangePasswordDto changePasswordDto);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFeedbackService _feedbackService;
        private readonly ICurrentUserService _currentUserService;
        public UserService(DataContext contex, IMapper mapper, IFeedbackService feedbackService, ICurrentUserService currentUserService)
        {
            _context = contex;
            _mapper = mapper;
            _feedbackService = feedbackService;
            _currentUserService = currentUserService;
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

        public async Task<PaginatedResult<UserDto>> GetAllAsync(
            int pageNumber, 
            int pageSize, 
            string searchQuery = null,
            StatusEnum status = 0)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (status != 0)
                {
                    query = query.Where(u => u.Status == status);
                }

                // Search query filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        EF.Functions.Like(a.FullName, $"%{searchQuery}%") ||
                        EF.Functions.Like(a.Email, $"%{searchQuery}%"));
                }

                var totalUsers = await query.CountAsync();

                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = _mapper.Map<List<UserDto>>(users);

                userDtos.ForEach(async user =>
                {
                    user.ratings = _feedbackService.GetAverageRatingByUserId(user.Id);
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
                userDto.ratings = _feedbackService.GetAverageRatingByUserId(id);
                return userDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public User FindUserByEmailAsync(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == email);
                return user;
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

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto userDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }

                _mapper.Map(userDto, user);
                await _context.SaveChangesAsync();
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<int> GetUserRoleByIdAsync(int id)
        {
            var role = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => u.Role)
            .FirstOrDefaultAsync();

            return role;
        }

        public async Task<User> CreateGoogleUserAsync(CreateUserDto userDto)
        {
            try
            {
                User user = _mapper.Map<User>(userDto);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return _context.Users.FirstOrDefault(x => x.Email == user.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetMe()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId);
                return _mapper.Map<UserDto>(user);
            } catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (string.IsNullOrEmpty(changePasswordDto.CurrentPassword))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Please provide old password");
                }

                if (string.IsNullOrEmpty(changePasswordDto.NewPassword))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Please provide new password");
                }

                var user = await _context.Users.FindAsync(_currentUserService.UserId);
                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.Password))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Old password is incorrect");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                user.Password = hashedPassword;
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                throw;
            }
        }
    }
}
