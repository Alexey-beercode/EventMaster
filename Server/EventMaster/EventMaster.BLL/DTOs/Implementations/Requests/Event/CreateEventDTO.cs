using EventMaster.BLL.DTOs.Implementations.BaseDTOs;
using EventMaster.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace EventMaster.BLL.DTOs.Implementations.Requests.Event;

public class CreateEventDTO : EventBaseDTO<CreateEventDTO>
{
    public IFormFile Image { get; set; }
}