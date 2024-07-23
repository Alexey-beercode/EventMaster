using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace EventMaster.DAL.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IParticipantRepository _participantRepository;

        public UnitOfWork(ApplicationDbContext dbContext,
            IEventRepository eventRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IEventCategoryRepository eventCategoryRepository,
            IParticipantRepository participantRepository)
        {
            _dbContext = dbContext;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _participantRepository = participantRepository;
        }

        public IEventRepository Events => _eventRepository;

        public IUserRepository Users => _userRepository;

        public IRoleRepository Roles => _roleRepository;

        public IEventCategoryRepository EventCategories => _eventCategoryRepository;

        public IParticipantRepository Participants => _participantRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}