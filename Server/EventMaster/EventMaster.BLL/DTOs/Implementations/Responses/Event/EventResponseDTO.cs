using EventMaster.BLL.DTOs.Implementations.BaseDTOs;

namespace EventMaster.BLL.DTOs.Implementations.Responses.Event;

public class EventResponseDTO : EventBaseDTO<EventResponseDTO>
{
    public Guid Id { get; set; }
    public byte[] Image { get; set; }
}