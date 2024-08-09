using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.Domain.Entities;
using EventMaster.Domain.Models;

namespace EventMaster.BLL.Infrastructure.Mapper
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventResponseDTO>();
            
            CreateMap<CreateEventDTO, Event>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            
            CreateMap<UpdateEventDTO, Event>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()); 
            
            CreateMap<Event, EventUpdateEmail>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Date.ToString("f"))) 
                .ForMember(dest => dest.EventLocation, opt => opt.MapFrom(src => $"{src.Location.City} {src.Location.Street} {src.Location.Building}"))
                .ForMember(dest => dest.ToEmail, opt => opt.Ignore()); 
            
            CreateMap<Location, string>()
                .ConvertUsing(src => $"{src.City} {src.Street} {src.Building}");
            
        }
    }
}