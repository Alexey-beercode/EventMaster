using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Models;

namespace EventMaster.BLL.UseCases.Event;

public class UpdateEventUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public UpdateEventUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task ExecuteAsync(UpdateEventDTO updateEventDto, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(updateEventDto.Id, cancellationToken) ?? 
                          throw new EntityNotFoundException("Event", updateEventDto.Id);

        _mapper.Map(updateEventDto, eventEntity);

        if (updateEventDto.Image != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await updateEventDto.Image.CopyToAsync(memoryStream, cancellationToken);
                eventEntity.Image = memoryStream.ToArray();
            }
        }

        _unitOfWork.Events.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await NotifyParticipantsAsync(eventEntity.Id, cancellationToken);
    }

    private async Task NotifyParticipantsAsync(Guid eventId, CancellationToken cancellationToken = default)
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