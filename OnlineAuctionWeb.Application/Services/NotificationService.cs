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
        Task<List<NotificationDto>> GetListNotifications();
    }
    public class NotificationService : INotificationService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public NotificationService(DataContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<List<NotificationDto>> GetListNotifications()
        {
            try {
                var notifications = await _context.Notifications.Include(x => x.UserNotifications).Where(x => x.UserNotifications.Any(x => x.UserId == _currentUserService.UserId)).ToListAsync();
                return _mapper.Map<List<NotificationDto>>(notifications);
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task SendNotificationAsync(int userId, CreateNotificationDto notificationDto)
        {
            try {
                var notification = _mapper.Map<Notification>(notificationDto);
                await _context.Notifications.AddAsync(notification);
                await _context.UserNotifications.AddAsync(new UserNotification { UserId = userId, Notification = notification, IsRead = false });
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw;
            }
        }
    }
}
