using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IUserRepository:IBaseRepository<User>
{
    Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken);
    void Update(User entity);
    Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}