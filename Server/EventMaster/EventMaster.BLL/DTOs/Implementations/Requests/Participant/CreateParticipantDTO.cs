using EventMaster.BLL.DTOs.Implementations.BaseDTOs;

namespace EventMaster.BLL.DTOs.Implementations.Requests.Participant;

public class CreateParticipantDTO: ParticipantBaseDTO<CreateParticipantDTO>
{
    public DateTime BirthDate { get; set; }
    public Guid EventId { get; set; }
}