using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.DTOs.Responses.Participant;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;

namespace EventMaster.BLL.Services.Implementation
{
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ParticipantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateParticipantDTO participantDto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(participantDto.UserId, cancellationToken) 
                       ?? throw new EntityNotFoundException("User", participantDto.UserId);
            var participantEvent = await _unitOfWork.Events.GetByIdAsync(participantDto.EventId, cancellationToken) 
                                   ?? throw new EntityNotFoundException("Event", participantDto.EventId);

            var participant = _mapper.Map<Participant>(participantDto);
            participant.RegistrationDate = DateTime.UtcNow;

            await _unitOfWork.Participants.CreateAsync(participant, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid participantId, CancellationToken cancellationToken = default)
        {
            var participant = await _unitOfWork.Participants.GetByIdAsync(participantId, cancellationToken) 
                              ?? throw new EntityNotFoundException("Participant", participantId);
            _unitOfWork.Participants.Delete(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
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

        public async Task<IEnumerable<ParticipantDTO>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
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
}
