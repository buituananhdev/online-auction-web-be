using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OnlineAuctionWeb.Application.Utils;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Infrastructure.Exceptions;

namespace OnlineAuctionWeb.Application
{
    public interface IAuthService
    {
        Task<TokenPayload> Login(string username, string password);
        Task<Boolean> Register(RegisterPayload registerPayload);
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

        public async Task<TokenPayload> Login(string email, string password)
        {
            try
            {
                var user = await _userService.FindUserByEmailAsync(email);
                if (user == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "User not found!");
                }
                if (user.IsActive == StatusEnum.Inactive)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "User is inactive!");
                }
                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    throw new CustomException(StatusCodes.Status404NotFound, "Invalid credential!");
                }

                var tokenPayload = JwtUtil.GenerateAccessToken(user.Id, _configuration);
                return tokenPayload;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw new Exception();
            }
        }

        public async Task<Boolean> Register(RegisterPayload registerPayload)
        {
            try
            {
                if (registerPayload.Role == RoleEnum.Admin)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, "Can not register this role!");
                }
                return await _userService.CreateAsync(_mapper.Map<UserDto>(registerPayload));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw new Exception();
            }
        }
    }
}
