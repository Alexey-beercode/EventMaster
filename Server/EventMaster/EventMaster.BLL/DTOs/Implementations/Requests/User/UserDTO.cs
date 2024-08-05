namespace EventMaster.BLL.DTOs.Implementations.Requests.User;

public class UserDTO: BaseValidationModel<UserDTO>
{
    public string Login { get; set; }
    public string Password { get; set; }
}