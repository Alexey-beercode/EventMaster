using EventMaster.Domain.Entities.Interfaces;

namespace EventMaster.Domain.Entities.Implementations;

public class EventCategory:IBaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
}