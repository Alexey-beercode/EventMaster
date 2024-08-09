using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.DTOs.Responses.Participant;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Implementation;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using Moq;

namespace EventMaster.Tests.Services;

public class ParticipantServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ParticipantService _participantService;

    public ParticipantServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _participantService = new ParticipantService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesParticipantAndSavesChanges()
    {
        var participantDto = new CreateParticipantDTO
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Email = "john.doe@example.com",
            UserId = Guid.NewGuid(),
            EventId = Guid.NewGuid()
        };

        var user = new User { Id = participantDto.UserId };
        var participantEvent = new Event { Id = participantDto.EventId };
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            FirstName = participantDto.FirstName,
            LastName = participantDto.LastName,
            BirthDate = participantDto.BirthDate,
            Email = participantDto.Email,
            UserId = participantDto.UserId,
            EventId = participantDto.EventId,
            RegistrationDate = DateTime.UtcNow
        };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(participantDto.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participantEvent);
        _mapperMock.Setup(m => m.Map<Participant>(participantDto))
            .Returns(participant);
        _unitOfWorkMock.Setup(uow => uow.Participants.CreateAsync(participant, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        await _participantService.CreateAsync(participantDto);
        
        _unitOfWorkMock.Verify(uow => uow.Participants.CreateAsync(participant, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesParticipantAndSavesChanges()
    {
        var participantId = Guid.NewGuid();
        var participant = new Participant { Id = participantId };

        _unitOfWorkMock.Setup(uow => uow.Participants.GetByIdAsync(participantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participant);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        await _participantService.DeleteAsync(participantId);
        
        _unitOfWorkMock.Verify(uow => uow.Participants.Delete(participant), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEventIdAsync_ValidEventId_ReturnsParticipantDtos()
    {
        var eventId = Guid.NewGuid();
        var eventFromDb = new Event { Id = eventId };
        var participant1 = new Participant { Id = Guid.NewGuid(), EventId = eventId };
        var participant2 = new Participant { Id = Guid.NewGuid(), EventId = eventId };
        var participants = new List<Participant> { participant1, participant2 };

        var participantDto1 = new ParticipantDTO { Id = participant1.Id, Event = new EventResponseDTO() };
        var participantDto2 = new ParticipantDTO { Id = participant2.Id, Event = new EventResponseDTO() };
        var participantDtos = new List<ParticipantDTO> { participantDto1, participantDto2 };

        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventFromDb);
        _unitOfWorkMock.Setup(uow => uow.Participants.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        _mapperMock.Setup(m => m.Map<IEnumerable<ParticipantDTO>>(participants))
            .Returns(participantDtos);
        
        var result = await _participantService.GetByEventIdAsync(eventId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _unitOfWorkMock.Verify(uow => uow.Participants.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_ValidUserId_ReturnsParticipantDtos()
    {
        
        var userId = Guid.NewGuid();
        var participant1 = new Participant { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid() };
        var participant2 = new Participant { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid() };
        var participants = new List<Participant> { participant1, participant2 };

        var participantDto1 = new ParticipantDTO { Id = participant1.Id, Event = new EventResponseDTO() };
        var participantDto2 = new ParticipantDTO { Id = participant2.Id, Event = new EventResponseDTO() };
        var participantDtos = new List<ParticipantDTO> { participantDto1, participantDto2 };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId });
        _unitOfWorkMock.Setup(uow => uow.Participants.GetByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        _mapperMock.Setup(m => m.Map<IEnumerable<ParticipantDTO>>(participants))
            .Returns(participantDtos);

        foreach (var participant in participants)
        {
            _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(participant.EventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = participant.EventId });
        }
        
        var result = await _participantService.GetByUserIdAsync(userId);

        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _unitOfWorkMock.Verify(uow => uow.Participants.GetByUserId(userId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Events.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_UserNotFound_ThrowsEntityNotFoundException()
    {
        var participantDto = new CreateParticipantDTO { UserId = Guid.NewGuid(), EventId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.CreateAsync(participantDto));
    }

    [Fact]
    public async Task CreateAsync_EventNotFound_ThrowsEntityNotFoundException()
    {
        var participantDto = new CreateParticipantDTO { UserId = Guid.NewGuid(), EventId = Guid.NewGuid() };
        var user = new User { Id = participantDto.UserId };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(participantDto.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.CreateAsync(participantDto));
    }

    [Fact]
    public async Task DeleteAsync_ParticipantNotFound_ThrowsEntityNotFoundException()
    {
        
        var participantId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Participants.GetByIdAsync(participantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Participant)null);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.DeleteAsync(participantId));
    }

    [Fact]
    public async Task GetByEventIdAsync_EventNotFound_ThrowsEntityNotFoundException()
    {
        
        var eventId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null);

        
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.GetByEventIdAsync(eventId));
    }
}