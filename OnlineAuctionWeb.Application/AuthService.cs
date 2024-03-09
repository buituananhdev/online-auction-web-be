using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OnlineAuctionWeb.Infrastructure.Utils;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Infrastructure.Exceptions;

namespace OnlineAuctionWeb.Application
{
    public interface IAuthService
    {
        Task<TokenPayload> Login(LoginDto loginDto);
        Task Register(RegisterDto registerDto);
    }
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthService(IUserService userService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<TokenPayload> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userService.FindUserByEmailAsync(loginDto.Email);
                if(user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }
                if (user.IsActive == StatusEnum.Inactive)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "User is inactive!");
                }
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Invalid credential!");
                }

                var tokenPayload = JwtUtil.GenerateAccessToken(user, _configuration);
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
