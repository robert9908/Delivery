using AutoMapper;
using Delivery.Models.Domain;
using Delivery.Models.DTO;

namespace Delivery.Mapping
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<AddOrderRequestDto, Order>().ReverseMap();
            CreateMap<UpdateOrderRequestDto, Order>().ReverseMap();
        }
    }
}
