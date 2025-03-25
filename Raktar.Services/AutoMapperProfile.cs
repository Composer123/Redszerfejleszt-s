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
            CreateMap<SimpleAddress, SimpleAddressDTO>().ReverseMap();
            CreateMap<LandRegistryNumber, LandRegistryNumberDTO>().ReverseMap();
        }
    }
}
