using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IParticipantRepository:IBaseRepository<Participant>
{
    void Update(Participant entity);
    Task<IEnumerable<Participant>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
}