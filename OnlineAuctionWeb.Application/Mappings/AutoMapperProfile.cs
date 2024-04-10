using AutoMapper;
using OnlineAuctionWeb.Domain.Dtos;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User profile
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, RegisterDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<UserDto, UpdateUserDto>().ReverseMap();

            // Auction profile
            CreateMap<Auction, AuctionDto>().ReverseMap();
            CreateMap<CreateAuctionDto, Auction>().ReverseMap();

            // Category profile
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryDto, UpdateCategoryDto>().ReverseMap();

            // Bid profile
            CreateMap<Bid, BidDto>().ReverseMap();
            CreateMap<Bid, CreateBidDto>().ReverseMap();

            // WatchList profile
            CreateMap<WatchList, CreateWatchListDto>().ReverseMap();
        }
    }
}
