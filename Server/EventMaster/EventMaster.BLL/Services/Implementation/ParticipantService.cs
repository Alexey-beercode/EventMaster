using AutoMapper;
using EventMaster.BLL.DTOs.Requests.Participant;
using EventMaster.BLL.DTOs.Responses.Participant;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.Services.Implementation;

public class ParticipantService:IParticipantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ParticipantService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task CreateAsync(CreateParticipantDTO participantDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid participantId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ParticipantDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ParticipantDTO>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}