namespace EventMaster.BLL.DTOs.Implementations.Requests.UserRole;

public class UserRoleDTO: BaseValidationModel<UserRoleDTO>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}