using AutoMapper;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Domain.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User profile
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, RegisterDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();

            // Auction profile
            CreateMap<Auction, AuctionDto>().ReverseMap();
            CreateMap<CreateAuctionDto, Auction>().ReverseMap();

            // Category profile
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();

            // Bid profile
            CreateMap<Bid, BidDto>().ReverseMap();
            CreateMap<Bid, CreateBidDto>().ReverseMap();
        }
    }
}
