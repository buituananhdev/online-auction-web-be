using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OnlineAuctionWeb.Application.Exceptions;
using OnlineAuctionWeb.Application.Services;
using OnlineAuctionWeb.Infrastructure.Hubs.Schemas;
using System.Collections.Concurrent;

namespace OnlineAuctionWeb.Infrastructure.Hubs
{
    public class AuctionHub : Hub
    {
        public const string USER_JOIN_AUCTION = "USER_JOIN_AUCTION";
        public const string USER_LEAVE_AUCTION = "USER_LEAVE_AUCTION";

        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly IWatchListService _watchListService;
        private readonly ICurrentUserService _currentUserService;
        public static readonly ConcurrentDictionary<int, HubUser> Users = new();

        public AuctionHub(IWatchListService watchListService, IHubContext<AuctionHub> hubContext)
        {
            _watchListService = watchListService;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                if (_currentUserService.UserId == null)
                {
                    throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token!");
                }

                Console.WriteLine("User connected");
                int userId = (int)_currentUserService.UserId;
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
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Auction-{auction}");
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
                var userId = (int)_currentUserService.UserId;
                var connectionId = Context.ConnectionId;
                var userConnection = Users.GetOrAdd(
                    userId,
                    new HubUser { UserId = userId, ConnectionIds = new HashSet<string>() }
                );
                lock (userConnection.ConnectionIds)
                {
                    userConnection.ConnectionIds.Remove(connectionId);
                }

                var watchList = await _watchListService.GetListAuctionIdsByUserIDAsync(userId);
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
