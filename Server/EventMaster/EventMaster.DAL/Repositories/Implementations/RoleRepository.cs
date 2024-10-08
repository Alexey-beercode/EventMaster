using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class RoleRepository:BaseRepository<Role>,IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.UsersRoles
            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.UsersRoles.AnyAsync(ur => ur.RoleId == roleId && !ur.IsDeleted,cancellationToken);
    }

    public async Task<bool> SetRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (user == null || role == null)
        {
            return false;
        }

        var isExists =
            await _dbContext.UsersRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        if (isExists)
        {
            return false;
        }

        var userRole = new UserRole { UserId = userId, RoleId = roleId };
        await _dbContext.UsersRoles.AddAsync(userRole, cancellationToken);
        return true;
    }

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _dbContext.UsersRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole == null)
        {
            return false;
        }

        userRole.IsDeleted = true;
        _dbContext.UsersRoles.Update(userRole);
        return true;
    }
}