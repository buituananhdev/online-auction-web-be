using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OnlineAuctionWeb.Infrastructure.Utils;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Application.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using OnlineAuctionWeb.Domain;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IAuthService
    {
        Task<TokenPayload> Login(LoginDto loginDto);
        Task Register(RegisterDto registerDto);
        Task<TokenPayload> AuthWithGoogle(string googleToken, RoleEnum role);
    }
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public AuthService(IUserService userService, IConfiguration configuration, IMapper mapper, DataContext context)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public async Task<TokenPayload> AuthWithGoogle(string googleToken, RoleEnum role)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtTokenObject = tokenHandler.ReadJwtToken(googleToken);
            string email = jwtTokenObject.Claims.First(claim => claim.Type == "email").Value;

            var existingUser = _userService.FindUserByEmailAsync(email);
            if (existingUser == null)
            {
                if (role == RoleEnum.Admin || !Enum.IsDefined(typeof(RoleEnum), role))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Invalid role specified!");
                }
                string name = jwtTokenObject.Claims.First(claim => claim.Type == "name").Value;
                string picture = jwtTokenObject.Claims.First(claim => claim.Type == "picture").Value;
                var user = new User
                {
                    Email = email,
                    FullName = name,
                    Avatar = picture,
                    Role = (int)role,
                    Status = StatusEnum.Active
                };

                existingUser = await _userService.CreateGoogleUserAsync(_mapper.Map<CreateUserDto>(user));
            }

            var tokenPayload = JwtUtil.GenerateAccessToken(_mapper.Map<UserDto>(existingUser), _configuration);
            return tokenPayload;
        }

        public async Task<TokenPayload> Login(LoginDto loginDto)
        {
            try
            {
                var user = _userService.FindUserByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }
                if (user.Status == StatusEnum.Inactive)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "User is inactive!");
                }
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Invalid credential!");
                }

                var tokenPayload = JwtUtil.GenerateAccessToken(_mapper.Map<UserDto>(user), _configuration);
                return tokenPayload;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw;
            }
        }

        public async Task Register(RegisterDto registerDto)
        {
            try
            {
                if (registerDto.Role == RoleEnum.Admin || !Enum.IsDefined(typeof(RoleEnum), registerDto.Role))
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, "Invalid role specified!");
                }

                await _userService.CreateAsync(_mapper.Map<CreateUserDto>(registerDto));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw;
            }
        }
    }
}
