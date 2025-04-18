using AutoMapper;
using Raktar.DataContext.Entities;
using Raktar.DataContext.DataTransferObjects;

namespace Raktar.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, User>();
            CreateMap<UserUpdateDTO, User>();

            CreateMap<Address, IAddressDTO>().ReverseMap();
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
            CreateMap<FeedbackCreateDTO, Feedback>();


            CreateMap<SimpleAddress, SimpleAddressDTO>().ReverseMap();
            CreateMap<LandRegistryNumber, LandRegistryNumberDTO>().ReverseMap();
            
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>();

            CreateMap<Order, OrderDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<OrderCreateDTO, Order>();
        }
    }
}
