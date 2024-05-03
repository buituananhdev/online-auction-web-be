using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;
using OnlineAuctionWeb.Domain.Payloads;

namespace OnlineAuctionWeb.Application.Services
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<PaginatedResult<PaymentDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string searchQuery = null
        );
    }
    public class PaymentService : IPaymentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IBidService _bidService;
        public PaymentService(DataContext context, IMapper mapper, INotificationService notificationService, IBidService bidService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _bidService = bidService;
        }
        public async Task CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(createPaymentDto);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                var auction = await _bidService.GetAuctionByBidIDAsync(createPaymentDto.BidId);

                var notificationDto = new CreateNotificationDto
                {
                    Title = "New payment",
                    Content = $"The customer has paid for your auction",
                    RedirectUrl = $"/seller-history/{auction.Id}",
                    RelatedID = auction.Id,
                    Type = NotificationType.UpdatePrice,
                };
                await _notificationService.SendNotificationAsync(auction.User.Id, notificationDto);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<PaginatedResult<PaymentDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string searchQuery = null
        )
        {
            try
            {
                var query = _context.Payments
                    .Include(a => a.Bid)
                    .ThenInclude(a => a.Auction)
                    .ThenInclude(a => a.User)
                    .AsQueryable();

                // Filter by search query (fuzzy search)
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(a =>
                        EF.Functions.Like(a.TransactionNumber, $"%{searchQuery}%"));
                }

                var totalPayments = await query.CountAsync();

                var payments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalPayments / pageSize);

                // Map payments to AuctionDtos
                var paymentDtos = new List<PaymentDto>();
                foreach (var payment in payments)
                {
                    var paymentDto = _mapper.Map<PaymentDto>(payment);
                    paymentDto.Auction = _mapper.Map<AuctionDto>(payment.Bid.Auction);
                    paymentDto.Auction.User = _mapper.Map<UserDto>(payment.Bid.Auction.User);
                    paymentDto.Bid = _mapper.Map<BidDto>(payment.Bid);
                    paymentDtos.Add(paymentDto);
                }

                var meta = new PaginatedMeta
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                };

                var result = new PaginatedResult<PaymentDto>
                {
                    Meta = meta,
                    Data = paymentDtos
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err", ex.Message);
                throw;
            }
        }
    }
}
