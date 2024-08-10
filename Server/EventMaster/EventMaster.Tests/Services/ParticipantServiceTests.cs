using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.DTOs.Implementations.Responses.Participant;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.UseCases.Participant;
using EventMaster.DAL.Infrastructure;
using Moq;

namespace EventMaster.Tests.Services
{
    public class ParticipantServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateParticipantUseCase _createParticipantUseCase;
        private readonly DeleteParticipantUseCase _deleteParticipantUseCase;
        private readonly GetParticipantsByEventIdUseCase _getParticipantsByEventIdUseCase;
        private readonly GetParticipantsByUserIdUseCase _getParticipantsByUserIdUseCase;
        private readonly ParticipantService _participantService;

        public ParticipantServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _createParticipantUseCase = new CreateParticipantUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            _deleteParticipantUseCase = new DeleteParticipantUseCase(_unitOfWorkMock.Object);
            _getParticipantsByEventIdUseCase = new GetParticipantsByEventIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            _getParticipantsByUserIdUseCase = new GetParticipantsByUserIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            _participantService = new ParticipantService(
                _createParticipantUseCase,
                _deleteParticipantUseCase,
                _getParticipantsByEventIdUseCase,
                _getParticipantsByUserIdUseCase
            );
        }

        [Fact]
        public async Task CreateAsync_ValidDto_CreatesParticipant()
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

            var participant = new Domain.Entities.Participant();
            _mapperMock.Setup(m => m.Map<Domain.Entities.Participant>(participantDto)).Returns(participant);

            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.User());

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(participantDto.EventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Event());

            _unitOfWorkMock.Setup(u => u.Participants.CreateAsync(participant, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _participantService.CreateAsync(participantDto);

            _unitOfWorkMock.Verify(u => u.Participants.CreateAsync(participant, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesParticipant()
        {
            var participantId = Guid.NewGuid();
            var participant = new Domain.Entities.Participant { Id = participantId };

            _unitOfWorkMock.Setup(u => u.Participants.GetByIdAsync(participantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(participant);

            _unitOfWorkMock.Setup(u => u.Participants.Delete(participant));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _participantService.DeleteAsync(participantId);

            _unitOfWorkMock.Verify(u => u.Participants.Delete(participant), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByEventIdAsync_ValidEventId_ReturnsParticipantDtos()
        {
            var eventId = Guid.NewGuid();
            var participants = new List<Domain.Entities.Participant>
            {
                new Domain.Entities.Participant { Id = Guid.NewGuid(), EventId = eventId },
                new Domain.Entities.Participant { Id = Guid.NewGuid(), EventId = eventId }
            };

            var participantDtos = new List<ParticipantDTO>
            {
                new ParticipantDTO { Id = participants[0].Id, Event = new EventResponseDTO() },
                new ParticipantDTO { Id = participants[1].Id, Event = new EventResponseDTO() }
            };

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Event());

            _unitOfWorkMock.Setup(u => u.Participants.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(participants);

            _mapperMock.Setup(m => m.Map<IEnumerable<ParticipantDTO>>(participants))
                .Returns(participantDtos);

            _mapperMock.Setup(m => m.Map<EventResponseDTO>(It.IsAny<Domain.Entities.Event>()))
                .Returns(new EventResponseDTO());

            var result = await _participantService.GetByEventIdAsync(eventId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _unitOfWorkMock.Verify(u => u.Participants.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByUserIdAsync_ValidUserId_ReturnsParticipantDtos()
        {
            var userId = Guid.NewGuid();
            var participants = new List<Domain.Entities.Participant>
            {
                new Domain.Entities.Participant { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid() },
                new Domain.Entities.Participant { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid() }
            };

            var participantDtos = new List<ParticipantDTO>
            {
                new ParticipantDTO { Id = participants[0].Id, Event = new EventResponseDTO() },
                new ParticipantDTO { Id = participants[1].Id, Event = new EventResponseDTO() }
            };

            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.User());

            _unitOfWorkMock.Setup(u => u.Participants.GetByUserId(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(participants);

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Event());

            _mapperMock.Setup(m => m.Map<IEnumerable<ParticipantDTO>>(participants))
                .Returns(participantDtos);

            _mapperMock.Setup(m => m.Map<EventResponseDTO>(It.IsAny<Domain.Entities.Event>()))
                .Returns(new EventResponseDTO());

            var result = await _participantService.GetByUserIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _unitOfWorkMock.Verify(u => u.Participants.GetByUserId(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_UserNotFound_ThrowsEntityNotFoundException()
        {
            var participantDto = new CreateParticipantDTO { UserId = Guid.NewGuid(), EventId = Guid.NewGuid() };

            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.User)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.CreateAsync(participantDto));
        }

        [Fact]
        public async Task CreateAsync_EventNotFound_ThrowsEntityNotFoundException()
        {
            var participantDto = new CreateParticipantDTO { UserId = Guid.NewGuid(), EventId = Guid.NewGuid() };

            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(participantDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.User());

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(participantDto.EventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Event)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.CreateAsync(participantDto));
        }

        [Fact]
        public async Task DeleteAsync_ParticipantNotFound_ThrowsEntityNotFoundException()
        {
            var participantId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.Participants.GetByIdAsync(participantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Participant)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.DeleteAsync(participantId));
        }

        [Fact]
        public async Task GetByEventIdAsync_EventNotFound_ThrowsEntityNotFoundException()
        {
            var eventId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Event)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _participantService.GetByEventIdAsync(eventId));
        }
    }
}
