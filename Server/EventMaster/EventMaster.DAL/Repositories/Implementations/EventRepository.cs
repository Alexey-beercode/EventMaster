using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using EventMaster.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class EventRepository: BaseRepository<Event>, IEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
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

    
    public async Task<IQueryable<Event>> GetEventsQueryableAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_dbContext.Events.Where(e => !e.IsDeleted).AsQueryable());
    }


}