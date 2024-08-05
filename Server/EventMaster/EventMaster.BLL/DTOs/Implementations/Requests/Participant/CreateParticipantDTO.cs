namespace EventMaster.BLL.DTOs.Implementations.Requests.Participant;

public class CreateParticipantDTO: BaseValidationModel<CreateParticipantDTO>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}