namespace EventMaster.BLL.DTOs.Requests.Participant;

public class CreateParticipantDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}