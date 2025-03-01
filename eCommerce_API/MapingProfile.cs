using AutoMapper;
using eCommerce_API.DTO;
using eCommerce_API.Models;
using eCommerce_API.Service;

namespace eCommerce_API
{
    public class MapingProfile:Profile
    {  
        public MapingProfile()
        {                           
            CreateMap<Product, ProductDTO>()
               .ForMember(x => x.image, opt => opt.MapFrom(src =>  src.image))
               .ForMember(x => x.category, opt => opt.MapFrom(src => src.category.name));
            CreateMap<Category, CategoryDTO>()
                .ForMember(x => x.products, opt => opt.MapFrom(src => src.products));
        }
    }
}
