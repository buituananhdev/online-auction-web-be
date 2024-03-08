using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain.Models;
using System;
using OnlineAuctionWeb.Domain.Enums;

namespace OnlineAuctionWeb.Application
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<Boolean> CreateAsync(UserDto userDto);
        Task<UserDto> UpdateAsync(UserDto userDto);
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
                throw new CustomeException(ex.Message);
            }
        }

        public Task<UserDto> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if(user == null)
                {
                    throw new CustomeException("User not found!");
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                return null;
            }
        }

        public async Task<UserDto> FindUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    return null;
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                return null;
            }
        }

        public Task<UserDto> UpdateAsync(UserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
