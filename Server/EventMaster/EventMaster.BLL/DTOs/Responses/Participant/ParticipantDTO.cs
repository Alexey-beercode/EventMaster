namespace EventMaster.BLL.DTOs.Responses.Participant;

public class ParticipantDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Domain.Entities.Implementations.Event Event { get; set; }
}