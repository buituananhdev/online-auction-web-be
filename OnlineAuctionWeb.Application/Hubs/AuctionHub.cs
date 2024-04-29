using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Application.Hubs.Schemas;
using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace OnlineAuctionWeb.Application.Hubs
{
    public sealed class AuctionHub : Hub
    {
        public const string ERROR = "ERROR";
        public const string USER_JOIN_AUCTION = "USER_JOIN_AUCTION";
        public const string USER_LEAVE_AUCTION = "USER_LEAVE_AUCTION";
        public const string RECEIVE_NOTIFICATION = "RECEIVE_NOTIFICATION";

        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly IWatchListService _watchListService;
        private readonly ICurrentUserService _currentUserService;
        public static readonly ConcurrentDictionary<int?, HubUser> Users = new();

        public AuctionHub(IWatchListService watchListService, IHubContext<AuctionHub> hubContext)
        {
            _watchListService = watchListService;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var configuration = httpContext.RequestServices.GetService<IConfiguration>();
                var token = httpContext.Request.Query["access_token"].ToString();
                Console.WriteLine(token);
                var tmp = Context.ConnectionId;
                if (string.IsNullOrEmpty(token))
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Secret"]);
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    },
                    out SecurityToken validatedToken
                );

                var jwtToken =
                    (JwtSecurityToken)validatedToken ?? throw new UnauthorizedAccessException();
                var claims = jwtToken.Claims;
                var userId = int.Parse(
                    claims.FirstOrDefault(x => x.Type == "ID")?.Value
                );

                Console.WriteLine("User connected");
                //int userId = (int)_currentUserService.UserId;
                var watchList = await _watchListService.GetListAuctionIdsByUserIDAsync(userId);

                var userConnection = Users.GetOrAdd(
                        userId,
                        new HubUser { UserId = userId, ConnectionIds = new HashSet<string>() }
                );

                lock (userConnection.ConnectionIds)
                {
                    userConnection.ConnectionIds.Add(Context.ConnectionId);
                }

                foreach (var auction in watchList)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, auction);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void SendMessage(string message)
        {
            Clients.All.SendAsync(message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }
                var userId = _currentUserService.UserId;
                var connectionId = Context.ConnectionId;
                var userConnection = Users.GetOrAdd(
                    userId,
                    new HubUser { UserId = userId, ConnectionIds = new HashSet<string>() }
                );
                lock (userConnection.ConnectionIds)
                {
                    userConnection.ConnectionIds.Remove(connectionId);
                }

                var watchList = await _watchListService.GetListAuctionIdsByUserIDAsync((int)userId);
                foreach (var auctionId in watchList)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, auctionId.ToString());
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UserJoinAuctionAsync(int auctionId, List<int> userIds)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    Users.TryGetValue(userId, out var hubUser);
                    if (hubUser is not null && hubUser.ConnectionIds.Any())
                    {
                        await Clients
                            .Clients(hubUser.ConnectionIds.ToList())
                            .SendAsync(USER_JOIN_AUCTION, auctionId);

                        foreach (var connectionId in hubUser.ConnectionIds)
                        {
                            await _hubContext.Groups.AddToGroupAsync(
                                connectionId,
                                auctionId.ToString()
                            );
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public async Task UserLeaveAuctionAsync(Guid channelId, List<int> userIds)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    Users.TryGetValue(userId, out var hubUser);
                    if (hubUser is not null && hubUser.ConnectionIds.Any())
                    {
                        await Clients
                            .Clients(hubUser.ConnectionIds.ToList())
                            .SendAsync(USER_LEAVE_AUCTION, channelId);

                        foreach (var connectionId in hubUser.ConnectionIds)
                        {
                            await _hubContext.Groups.RemoveFromGroupAsync(
                                connectionId,
                                channelId.ToString()
                            );
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public static Task<IEnumerable<string>> GetConnectionsByUserId(int userId)
        {
            Users.TryGetValue(userId, out var hubUser);
            return Task.FromResult(
                hubUser?.ConnectionIds.AsEnumerable() ?? Enumerable.Empty<string>()
            );
        }

        public async Task UpdatePriceAsync(int auctionId, decimal price)
        {
            await Clients.Group($"Auction-{auctionId}").SendAsync("BidPlaced", auctionId, price);
        }
    }
}
