using EventMaster.Domain.Entities.Interfaces;

namespace EventMaster.Domain.Entities.Implementations;

public class User:IBaseEntity
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string RefreshToken { get; set; }
    public bool IsDeleted { get; set; }
}