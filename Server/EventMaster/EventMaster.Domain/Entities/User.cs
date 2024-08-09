namespace EventMaster.Domain.Entities;

public class User:BaseEntity
{
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}