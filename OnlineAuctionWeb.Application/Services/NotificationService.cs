using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, CreateNotificationDto notificationDto);
        Task SendAuctionNotificationAsync(int auctionId, int sellerId, CreateNotificationDto notificationDto);
        Task<List<NotificationDto>> GetListNotifications();
    }
    public class NotificationService : INotificationService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWatchListService _watchListService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHubService _hubService;
        public NotificationService(DataContext context, IMapper mapper, ICurrentUserService currentUserService, IHubService hubService, IWatchListService watchListService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _hubService = hubService;
            _watchListService = watchListService;
        }

        public Task<List<NotificationDto>> GetListNotifications()
        {
            throw new NotImplementedException();
        }

        public async Task SendAuctionNotificationAsync(int auctionId, int sellerId, CreateNotificationDto notificationDto)
        {
            try
            {
                var notification = _mapper.Map<Notification>(notificationDto);
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                
                var userIds = await _watchListService.GetListUserIdsByAuctionIDAsync(auctionId);
                List<UserNotification> userNotifications = new List<UserNotification>();
                foreach(var user in userIds)
                {
                    var userNotification = new CreateUserNotificationsDto
                    {
                        UserId = int.Parse(user),
                        NotificationId = notification.Id,
                        IsRead = false
                    };

                    userNotifications.Add(_mapper.Map<UserNotification>(userNotification));
                }

                _context.UserNotifications.AddRange(userNotifications);
                await _context.SaveChangesAsync();

                _hubService.SendNotification(sellerId, _mapper.Map<NotificationDto>(notification));
                _hubService.SendGroupNotification(auctionId, _mapper.Map<NotificationDto>(notification));
            } catch(Exception ex)
            {
                throw;
            }
        }

        public Task SendNotificationAsync(int userId, CreateNotificationDto notificationDto)
        {
            throw new NotImplementedException();
        }
    }
}
