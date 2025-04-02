using AutoMapper;
using Raktar.DataContext.Entities;
using Raktar.DataContext.DataTransferObjects;
using static Raktar.DataContext.DataTransferObjects.ProductServiceDTO;
using ProductDTO = Raktar.DataContext.DataTransferObjects.ProductDTO;

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
            CreateMap<SimpleAddress, SimpleAddressDTO>().ReverseMap();
            CreateMap<LandRegistryNumber, LandRegistryNumberDTO>().ReverseMap();
            
            #region Product Service auto map
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>();
            #endregion

            #region Order Service auto map
            CreateMap<Order, OrderDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<OrderCreateDTO, Order>();
            #endregion

        }
    }
}
