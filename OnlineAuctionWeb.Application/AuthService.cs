using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Payloads;
using OnlineAuctionWeb.Application.Utils;
using Microsoft.Extensions.Configuration;
using OnlineAuctionWeb.Domain.Dtos;
using AutoMapper;

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
        public AuthService(DataContext context, IUserService userService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<TokenPayload> Login(string email, string password)
        {
            var user = await _userService.FindUserByEmailAsync(email);
            if (user == null)
            {
                throw new CustomeException("User not found!");
            }
            if(user.IsActive == StatusEnum.Inactive)
            {
                throw new CustomeException("User is inactive!");
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new CustomeException("Invalid password!");
            }

            var tokenPayload = JwtUtil.GenerateAccessToken(user.Id, _configuration);
            return tokenPayload;
        }

        public async Task<Boolean> Register(RegisterPayload registerPayload)
        {
            try
            {
                return await _userService.CreateAsync(_mapper.Map<UserDto>(registerPayload));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex);
                throw new CustomeException(ex.Message);
            }
        }
    }
}
