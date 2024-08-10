using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Participant;

public class CreateParticipantUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateParticipantUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(CreateParticipantDTO participantDto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(participantDto.UserId, cancellationToken) 
                   ?? throw new EntityNotFoundException("User", participantDto.UserId);
        var participantEvent = await _unitOfWork.Events.GetByIdAsync(participantDto.EventId, cancellationToken) 
                               ?? throw new EntityNotFoundException("Event", participantDto.EventId);

        var participant = _mapper.Map<Domain.Entities.Participant>(participantDto);
        participant.RegistrationDate = DateTime.UtcNow;

        await _unitOfWork.Participants.CreateAsync(participant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}