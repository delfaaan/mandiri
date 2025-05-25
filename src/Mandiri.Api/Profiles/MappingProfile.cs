using AutoMapper;
using Mandiri.Api.Controllers;
using Mandiri.Domain.Entities;

namespace Mandiri.Api.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<UserRequest, User>();
			CreateMap<User, UserResponse>();

			CreateMap<UserProfileRequest, UserProfile>();
			CreateMap<UserProfile, UserProfileResponse>();

			CreateMap<OrderRequest, Order>();
			CreateMap<Order, OrderResponse>().ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

			CreateMap<ProductRequest, Product>();
			CreateMap<Product, ProductResponse>().ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

			CreateMap<OrderProductRequest, OrderProduct>();
			CreateMap<OrderProductUpdateRequest, OrderProduct>().ForMember(dest => dest.ProductId, opt => opt.Ignore()).ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
			CreateMap<OrderProduct, OrderProductResponse>().ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));
		}
	}
}