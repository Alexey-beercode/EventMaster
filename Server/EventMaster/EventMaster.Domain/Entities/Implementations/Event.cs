using EventMaster.Domain.Entities.Interfaces;

namespace EventMaster.Domain.Entities.Implementations;

public class Event:IBaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int MaxParticipants { get; set; }
    public byte[] Image { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsDeleted { get; set; }
}