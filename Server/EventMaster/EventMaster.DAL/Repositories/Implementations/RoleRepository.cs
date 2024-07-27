using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class RoleRepository:IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.Roles.Where(r => !r.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Role entity, CancellationToken cancellationToken=default)
    {
        await _dbContext.Roles.AddAsync(entity, cancellationToken);
    }

    public void Delete(Role entity)
    {
        entity.IsDeleted = true;
        _dbContext.Roles.Update(entity);
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

    public async Task SetRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (user == null || role == null)
        {
            throw new InvalidOperationException("Role or user are not found");
        }

        var isExists =
            await _dbContext.UsersRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        if (isExists)
        {
            throw new InvalidOperationException($"This user has role with id : {roleId}");
        }

        var userRole = new UserRole { UserId = userId, RoleId = roleId };
        await _dbContext.UsersRoles.AddAsync(userRole, cancellationToken);
    }

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _dbContext.UsersRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole == null)
        {
            throw new InvalidOperationException("User role not found");
        }

        userRole.IsDeleted = true;
        _dbContext.UsersRoles.Update(userRole);
    }
}