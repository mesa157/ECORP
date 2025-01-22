using AutoMapper;
using ProdcutCatalog.Entities;
using ProdcutCatalog.Models;

namespace ProdcutCatalog.ProfileS 
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
