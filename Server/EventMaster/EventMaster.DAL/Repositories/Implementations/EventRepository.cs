using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities.Implementations;
using EventMaster.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class EventRepository:IEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Event> GetByIdAsync(Guid id, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted,cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.Events.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Event entity, CancellationToken cancellationToken=default)
    {
        await _dbContext.Events.AddAsync(entity, cancellationToken);
    }

    public void Delete(Event entity)
    {
        entity.IsDeleted = true;
        _dbContext.Events.Update(entity);
    }

    public void Update(Event entity)
    {
        _dbContext.Events.Update(entity);
    }

    public async Task<IEnumerable<Event>> GetByNameAsync(string name, int pageNumber, int pageSize, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Events
            .Where(e => e.Name.Contains(name) && !e.IsDeleted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    
    public async Task<IQueryable<Event>> GetEventsQueryableAsync(CancellationToken cancellationToken=default)
    {
        return await Task.FromResult(_dbContext.Events.Where(e => !e.IsDeleted).AsQueryable());
    }

}