using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class EventRepository:IEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Event> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted,cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Event entity, CancellationToken cancellationToken)
    {
        await _dbContext.Events.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Event entity, CancellationToken cancellationToken)
    {
        entity.IsDeleted = true;
        _dbContext.Events.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event entity, CancellationToken cancellationToken)
    {
        _dbContext.Events.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Event> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == name && !e.IsDeleted,cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(e => e.Date == date && !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByLocationAsync(string location, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(e => e.Location == location && !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Events.Where(e => e.CategoryId == categoryId && !e.IsDeleted).ToListAsync(cancellationToken);
    }
}