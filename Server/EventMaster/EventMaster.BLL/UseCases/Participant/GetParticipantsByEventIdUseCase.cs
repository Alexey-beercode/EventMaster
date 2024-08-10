using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.BLL.DTOs.Implementations.Responses.Participant;
using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Participant;

public class GetParticipantsByEventIdUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetParticipantsByEventIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ParticipantDTO>> ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventFromDb = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken) 
                          ?? throw new EntityNotFoundException("Event", eventId);
        var participantsByEvent = await _unitOfWork.Participants.GetByEventIdAsync(eventId, cancellationToken);
        var participantDtos = _mapper.Map<IEnumerable<ParticipantDTO>>(participantsByEvent);

        foreach (var participantDto in participantDtos)
        {
            participantDto.Event = _mapper.Map<EventResponseDTO>(eventFromDb);
        }

        return participantDtos;
    }
}