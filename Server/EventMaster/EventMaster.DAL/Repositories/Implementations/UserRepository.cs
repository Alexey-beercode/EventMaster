using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class UserRepository:BaseRepository<User>,IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(User entity)
    {
        _dbContext.Users.Update(entity);
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted && u.RefreshTokenExpiryTime > DateTime.Now);
    }

    public async Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && !u.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.UsersRoles
            .Where(ur => ur.RoleId == roleId && !ur.IsDeleted)
            .Select(ur => ur.User)
            .ToListAsync(cancellationToken);
    }
}