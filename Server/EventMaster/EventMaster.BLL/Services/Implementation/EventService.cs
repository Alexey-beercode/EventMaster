using System.Text;
using AutoMapper;
using EventMaster.BLL.DTOs.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Models;

namespace EventMaster.BLL.Services.Implementation;

public class EventService:IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public EventService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _mapper = mapper;
    }

    public Task CreateAsync(EventDTO eventDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EventResponseDTO>> GetFilteredEventsAsync(EventFilterDto filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EventResponseDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(EventDTO eventDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid eventId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<EventResponseDTO> GetByIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    private async Task NotifyParticipantsAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var participants = await _unitOfWork.Participants.GetByEventIdAsync(eventId, cancellationToken);
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken);

        foreach (var participant in participants)
        {
            var email = _mapper.Map<EventUpdateEmail>(eventEntity);
            email.Name = $"{participant.FirstName} {participant.LastName}";
            email.ToEmail = participant.Email;
            await _emailService.SendEmailAsync(email);
        }
    }
}