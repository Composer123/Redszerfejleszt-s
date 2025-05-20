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
            // User mappings...
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Password)));
            CreateMap<UserUpdateDTO, User>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            // Feedback mappings...
            CreateMap<Feedback, FeedbackDTO>()
    // Use the first order item’s Order if available, otherwise return null.
    .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.OrderItems.FirstOrDefault() != null ? src.OrderItems.FirstOrDefault().Order : null));



            CreateMap<FeedbackCreateDTO, Feedback>();

            // SimpleAddress mappings...
            CreateMap<SimpleAddress, SimpleAddressDTO>()
                .ForMember(dest => dest.AddressId,
                           opt => opt.MapFrom(src => src.Address != null ? src.Address.AddressId : 0))
                .ReverseMap();
            CreateMap<SimpleAddressCreateDTO, SimpleAddress>();

            // LandRegistryNumber mappings...
            CreateMap<LandRegistryNumber, LandRegistryNumberDTO>().ReverseMap();
            CreateMap<LandRegistryNumberCreateDTO, LandRegistryNumber>();

            // Product mappings...
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>();

            // Order mappings
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DeliveryAdressId,
                           opt => opt.MapFrom(src => src.DeliveryAdress != null ? src.DeliveryAdress.AddressId : src.DeliveryAdressId));
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            // **Add this mapping for order items:**
            
            CreateMap<OrderItem, OrderItemDTO>()
    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
    .ReverseMap();

            CreateMap<AddOrderItemDTO, OrderItem>();

            // Role and Settlement mappings...
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore());

            CreateMap<Settlement, SettlementDTO>();
            CreateMap<SettlementDTO, Settlement>();
        }
    }
}
