using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.BLL.DTOs.Implementations.Responses.Participant;
using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Participant;

public class GetParticipantsByUserIdUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetParticipantsByUserIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ParticipantDTO>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken) 
                   ?? throw new EntityNotFoundException("User", userId);
        var participantsByUser = await _unitOfWork.Participants.GetByUserId(userId, cancellationToken);
        var participantDtos = _mapper.Map<IEnumerable<ParticipantDTO>>(participantsByUser);

        foreach (var participantDto in participantDtos)
        {
            var participantEvent = await _unitOfWork.Events.GetByIdAsync(participantDto.Event.Id, cancellationToken);
            participantDto.Event = _mapper.Map<EventResponseDTO>(participantEvent); 
        }

        return participantDtos;
    }
}
