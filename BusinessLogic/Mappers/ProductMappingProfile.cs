using AutoMapper;
using BusinessLogic.DTOs;
using DataAccess.Entities;

namespace BusinessLogic.Mappers;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName));

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock));

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID));
    }
}
