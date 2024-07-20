using EventMaster.Domain.Entities.Interfaces;

namespace EventMaster.Domain.Entities.Implementations;

public class UserRole:IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public User User { get; set; }
    public bool IsDeleted { get; set; }
}