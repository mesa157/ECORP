using AutoMapper;
using ShoppingBasket.Models;

namespace ShoppingBasket.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.ShoppingBaskett, ShoppingBasketDto>();
            CreateMap<BasketLine, BasketLineDto>();
        }
    }
}
