using AutoMapper;
using ProdcutCatalog.Entities;
using ProdcutCatalog.Models;
using System;


namespace ProdcutCatalog.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opts => opts.MapFrom(src => src.Category.Name));
        }
    }
}
