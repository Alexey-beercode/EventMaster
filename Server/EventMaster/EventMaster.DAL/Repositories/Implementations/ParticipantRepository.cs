using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Repositories.Implementations;

public class ParticipantRepository:BaseRepository<Participant>,IParticipantRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ParticipantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    

    public void Update(Participant entity)
    {
        _dbContext.Participants.Update(entity);
    }

    public async Task<IEnumerable<Participant>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Participants
            .Where(p => p.EventId == eventId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetByUserId(Guid userId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Participants
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}