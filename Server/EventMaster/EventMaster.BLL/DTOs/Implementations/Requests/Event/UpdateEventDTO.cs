using EventMaster.BLL.DTOs.Implementations.BaseDTOs;
using EventMaster.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace EventMaster.BLL.DTOs.Implementations.Requests.Event;

public class UpdateEventDTO: EventBaseDTO<UpdateEventDTO>
{
    public Guid Id { get; set; }
    public IFormFile Image { get; set; }
}