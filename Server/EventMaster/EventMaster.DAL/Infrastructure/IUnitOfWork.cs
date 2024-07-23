using EventMaster.DAL.Repositories.Interfaces;

namespace EventMaster.DAL.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    IEventRepository Events { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IEventCategoryRepository EventCategories { get; }
    IParticipantRepository Participants { get; }
}