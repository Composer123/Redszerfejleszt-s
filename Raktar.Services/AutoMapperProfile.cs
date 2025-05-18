using AutoMapper;
using Raktar.DataContext.Entities;
using Raktar.DataContext.DataTransferObjects;
using System.Text;

namespace Raktar.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, User>();
            CreateMap<UserUpdateDTO, User>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore());


            CreateMap<Address, IAddressDTO>().ReverseMap();
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
            CreateMap<FeedbackCreateDTO, Feedback>();


            CreateMap<SimpleAddress, SimpleAddressDTO>().ReverseMap();
            CreateMap<LandRegistryNumber, LandRegistryNumberDTO>().ReverseMap();
            
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>();

            CreateMap<Order, OrderDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<OrderCreateDTO, Order>().ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<AddOrderItemDTO, OrderItem>();

            CreateMap<Role, RoleDTO>();
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())  // don't expose the password
            .ForMember(dest => dest.Orders, opt => opt.Ignore()); // ignore orders to avoid cycles

            CreateMap<Role, RoleDTO>().ReverseMap();

            // Mapping from UserRegisterDTO (with a string Password) to User.
            // This converts the string to a byte array when storing the password.
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Password)));

        }
    }
}
