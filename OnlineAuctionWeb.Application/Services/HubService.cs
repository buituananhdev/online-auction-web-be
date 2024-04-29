using Microsoft.AspNetCore.SignalR;
using OnlineAuctionWeb.Application.Hubs;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IHubService
    {
        Task AddToGroupAsync(string connectionId, string groupName);
        Task RemoveFromGroupAsync(string connectionId, string groupName);
        Task SendAsync(IEnumerable<string> connectionIds, string method, object arg1);
        Task RemoveUsersFromGroupHub(int groupID, List<int> userIds);
        Task AddUsersToGroupHub(int groupID, List<int> userIds);
        Task SendNotification(NotificationDto message);
        Task SendGroupNotification(int groupId, NotificationDto message);
    }
    public class HubService : IHubService
    {
        private readonly IHubContext<AuctionHub> _auctionHub;

        public HubService(IHubContext<AuctionHub> chatHub)
        {
            _auctionHub = chatHub;
        }

        public async Task AddToGroupAsync(string connectionId, string groupName)
        {
            await _auctionHub.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task RemoveFromGroupAsync(string connectionId, string groupName)
        {
            await _auctionHub.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task SendAsync(IEnumerable<string> connectionIds, string method, object arg1)
        {
            await _auctionHub.Clients.Clients(connectionIds).SendAsync(method, arg1);
        }

        public async Task RemoveUsersFromGroupHub(int groupID, List<int> userIds)
        {
            foreach (var userId in userIds)
            {
                var connectionIds = await AuctionHub.GetConnectionsByUserId(userId);
                if (connectionIds is not null)
                {
                    await _auctionHub.Clients
                        .Clients(connectionIds)
                        .SendAsync(AuctionHub.USER_LEAVE_AUCTION, groupID);
                    foreach (var connectionId in connectionIds)
                    {
                        await _auctionHub.Groups.RemoveFromGroupAsync(
                            connectionId,
                            groupID.ToString()
                        );
                    }
                }
            }
        }

        public async Task AddUsersToGroupHub(int groupID, List<int> userIds)
        {
            foreach (var userId in userIds)
            {
                var connectionIds = await AuctionHub.GetConnectionsByUserId(userId);
                if (connectionIds is not null)
                {
                    foreach (var connectionId in connectionIds)
                    {
                        await AddToGroupAsync(connectionId, groupID.ToString());
                    }

                    await SendAsync(connectionIds, AuctionHub.USER_JOIN_AUCTION, groupID);
                }
            }
        }

        public async Task SendNotification(NotificationDto notification)
        {
            try
            {
                var connectionIds = await AuctionHub.GetConnectionsByUserId(notification.Id);
                if (connectionIds is not null)
                {
                    await _auctionHub.Clients
                        .Clients(connectionIds.ToList())
                        .SendAsync(AuctionHub.RECEIVE_NOTIFICATION, notification);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SendGroupNotification(int groupId, NotificationDto notification)
        {
            try
            {
                await _auctionHub.Clients.Group(groupId.ToString()).SendAsync(AuctionHub.RECEIVE_NOTIFICATION, notification);
                //await _auctionHub.Clients.All.SendAsync(AuctionHub.RECEIVE_NOTIFICATION, notification);
            } catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
