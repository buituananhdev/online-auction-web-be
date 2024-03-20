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
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, RegisterDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();

            CreateMap<Auction, AuctionDto>().ReverseMap();
            CreateMap<CreateAuctionDto, Auction>().ReverseMap();

            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
