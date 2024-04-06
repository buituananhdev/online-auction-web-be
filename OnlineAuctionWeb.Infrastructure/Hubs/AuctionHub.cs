using Microsoft.AspNetCore.SignalR;

namespace OnlineAuctionWeb.Infrastructure.Hubs
{
    public class AuctionHub : Hub
    {
        public AuctionHub()
        {

        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("User connected");
            await Clients.All.SendAsync("UserConnected1111", Context.ConnectionId);
            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Caller.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UserJoinAuctionAsync(int auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
            await Clients.Caller.SendAsync("JoinedAuction", auctionId);
        }

        public async Task UserLeaveAuctionAsync(int auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
            await Clients.Caller.SendAsync("LeftAuction", auctionId);
        }

        public async Task UpdatePriceAsync(int auctionId, decimal price)
        {
            await Clients.Group($"Auction-{auctionId}").SendAsync("BidPlaced", auctionId, price);
        }
    }
}
