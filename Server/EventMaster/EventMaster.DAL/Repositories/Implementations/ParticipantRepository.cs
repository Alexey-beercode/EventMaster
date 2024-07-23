using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class ParticipantRepository:IParticipantRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ParticipantRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Participant> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Participants.Where(p => !p.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Participant entity, CancellationToken cancellationToken)
    {
        await _dbContext.Participants.AddAsync(entity, cancellationToken);
    }

    public void Delete(Participant entity)
    {
        entity.IsDeleted = true;
        _dbContext.Participants.Update(entity);
    }

    public void Update(Participant entity)
    {
        _dbContext.Participants.Update(entity);
    }

    public async Task<IEnumerable<Participant>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participants
            .Where(p => p.EventId == eventId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}