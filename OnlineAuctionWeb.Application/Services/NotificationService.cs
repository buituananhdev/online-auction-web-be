using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, CreateNotificationDto notificationDto);
        Task NewBidNotification(AuctionDto auction, int sellerId);
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

        public async Task<List<NotificationDto>> GetListNotifications()
        {
            try
            {
                var notificationDtos = await _context.Notifications
                    .Include(x => x.UserNotifications)
                    .Where(a => a.UserNotifications.Any(un => un.UserId == _currentUserService.UserId))
                    .Select(notification => new NotificationDto
                    {
                        Id = notification.Id,
                        Title = notification.Title,
                        Content = notification.Content,
                        IsRead = notification.UserNotifications.Any(un => un.UserId == _currentUserService.UserId && un.IsRead)
                    })
                    .ToListAsync();

                return notificationDtos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task NewBidNotification(AuctionDto auction, int sellerId)
        {
            try
            {
                var buyerNotificationDto = new CreateNotificationDto
                {
                    Title = "Auction Update: New Bid Placed!",
                    Content = $"A new bid has been placed on the auction for {auction.ProductName}",
                    RedirectUrl = $"/auctions/{auction.Id}",
                    RelatedID = auction.Id,
                    Type = NotificationType.NewBid,
                };

                var sellerNotificationDto = new CreateNotificationDto
                {
                    Title = "Bid Alert: New Offer Received!",
                    Content = $"You've got a new bid on the {auction.ProductName} auction.",
                    RedirectUrl = $"/auctions/{auction.Id}",
                    RelatedID = auction.Id,
                    Type = NotificationType.NewBid,
                };

                var buyerNotification = _mapper.Map<Notification>(buyerNotificationDto);
                var sellerNotification = _mapper.Map<Notification>(sellerNotificationDto);

                _context.Notifications.AddRange(buyerNotification, sellerNotification);
                await _context.SaveChangesAsync();

                var userIds = await _watchListService.GetListUserIdsByAuctionIDAsync(auction.Id);

                var userNotifications = userIds.Select(userId => new CreateUserNotificationsDto
                {
                    UserId = int.Parse(userId),
                    NotificationId = buyerNotification.Id,
                    IsRead = false
                }).ToList();

                userNotifications.Add(new CreateUserNotificationsDto
                {
                    UserId = sellerId,
                    NotificationId = sellerNotification.Id,
                    IsRead = false
                });

                _context.UserNotifications.AddRange(userNotifications.Select(dto => _mapper.Map<UserNotification>(dto)));
                await _context.SaveChangesAsync();

                await Task.WhenAll(
                    _hubService.SendNotification(sellerId, _mapper.Map<NotificationDto>(sellerNotification)),
                    _hubService.SendGroupNotification(auction.Id, _mapper.Map<NotificationDto>(buyerNotification))
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendNotificationAsync(int userId, CreateNotificationDto notificationDto)
        {
            try
            {
                await _hubService.SendNotification(userId, _mapper.Map<NotificationDto>(notificationDto));
            } catch (Exception ex)
            {
                throw;
            }
        }
    }
}
