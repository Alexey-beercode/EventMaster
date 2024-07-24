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

    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Roles.Where(r => !r.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Role entity, CancellationToken cancellationToken)
    {
        await _dbContext.Roles.AddAsync(entity, cancellationToken);
    }

    public void Delete(Role entity)
    {
        entity.IsDeleted = true;
        _dbContext.Roles.Update(entity);
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UsersRoles
            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckHasUserRole(Guid roleId, CancellationToken cancellationToken)
    {
        return await _dbContext.UsersRoles.AnyAsync(ur => ur.RoleId == roleId && !ur.IsDeleted,cancellationToken);
    }
}