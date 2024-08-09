using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class EventCategoryRepository:IEventCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EventCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EventCategory> GetByIdAsync(Guid id, CancellationToken cancellationToken=default)
    {
        return await _dbContext.EventCategories.FirstOrDefaultAsync(ec => ec.Id == id && !ec.IsDeleted,cancellationToken);
    }

    public async Task<IEnumerable<EventCategory>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.EventCategories.Where(ec => !ec.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(EventCategory entity, CancellationToken cancellationToken=default)
    {
        await _dbContext.EventCategories.AddAsync(entity, cancellationToken);
    }

    public void Delete(EventCategory entity)
    {
        entity.IsDeleted = true;
        _dbContext.EventCategories.Update(entity);
    }
}