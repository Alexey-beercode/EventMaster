using EventMaster.BLL.DTOs.Implementations.BaseDTOs;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;

namespace EventMaster.BLL.DTOs.Implementations.Responses.Participant;

public class ParticipantDTO:ParticipantBaseDTO<ParticipantDTO>
{
    public Guid Id { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public EventResponseDTO Event { get; set; }
}