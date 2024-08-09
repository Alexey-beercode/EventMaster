namespace EventMaster.Domain.Entities;

public class Participant:BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}