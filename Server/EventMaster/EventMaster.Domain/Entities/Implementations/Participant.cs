using EventMaster.Domain.Entities.Interfaces;

namespace EventMaster.Domain.Entities.Implementations;

public class Participant:IBaseEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public bool IsDeleted { get; set; }
}