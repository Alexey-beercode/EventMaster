using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class UserRepository:IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users.Where(u => !u.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(User entity, CancellationToken cancellationToken=default)
    {
        await _dbContext.Users.AddAsync(entity, cancellationToken);
    }

    public void Delete(User entity)
    {
        entity.IsDeleted = true;
        _dbContext.Users.Update(entity);
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