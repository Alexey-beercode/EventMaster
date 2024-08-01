using AutoMapper;
using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Infrastructure.Mapper;

public class EventCategoryProfile:Profile
{
    public EventCategoryProfile()
    {
        CreateMap<EventCategory, EventCategoryDTO>();
    }
}