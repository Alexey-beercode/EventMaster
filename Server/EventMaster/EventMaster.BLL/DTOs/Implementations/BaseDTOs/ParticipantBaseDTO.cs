namespace EventMaster.BLL.DTOs.Implementations.BaseDTOs;

public class ParticipantBaseDTO<T>:BaseValidationModel<T>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
}