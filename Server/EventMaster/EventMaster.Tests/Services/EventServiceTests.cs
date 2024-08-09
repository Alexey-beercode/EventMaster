using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using EventMaster.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;

public class EventServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _emailServiceMock = new Mock<IEmailService>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IMemoryCache>();
        _eventService = new EventService(_unitOfWorkMock.Object, _emailServiceMock.Object, _mapperMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesEventAndSavesChanges()
    {
        var createEventDto = new CreateEventDTO
        {
            Name = "Event1",
            Description = "Description",
            Date = DateTime.UtcNow,
            Location = new Location { City = "City", Street = "Street", Building = "Building" },
            MaxParticipants = 100,
            CategoryId = Guid.NewGuid()
        };

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = createEventDto.Name,
            Description = createEventDto.Description,
            Date = createEventDto.Date,
            Location = createEventDto.Location,
            MaxParticipants = createEventDto.MaxParticipants,
            CategoryId = createEventDto.CategoryId
        };

        _mapperMock.Setup(m => m.Map<Event>(createEventDto)).Returns(eventEntity);
        _unitOfWorkMock.Setup(uow => uow.Events.CreateAsync(eventEntity, CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);
        
        await _eventService.CreateAsync(createEventDto);
        
        _unitOfWorkMock.Verify(uow => uow.Events.CreateAsync(eventEntity, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_FiltersByName_ReturnsFilteredEvents()
    {
        var filter = new EventFilterDto { Name = "Event1", PageNumber = 1, PageSize = 10 };
        var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Event1" } };
        var eventDtos = new List<EventResponseDTO> { new EventResponseDTO { Id = events.First().Id, Name = "Event1" } };

        _unitOfWorkMock.Setup(uow => uow.Events.GetByNameAsync(filter.Name, filter.PageNumber, filter.PageSize, CancellationToken.None))
                       .ReturnsAsync(events);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);
        
        var result = await _eventService.GetFilteredEventsAsync(filter, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Event1", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEvents()
    {
        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Name = "Event1" },
            new Event { Id = Guid.NewGuid(), Name = "Event2" }
        };
        var eventDtos = new List<EventResponseDTO>
        {
            new EventResponseDTO { Id = events[0].Id, Name = "Event1" },
            new EventResponseDTO { Id = events[1].Id, Name = "Event2" }
        };

        _unitOfWorkMock.Setup(uow => uow.Events.GetAllAsync(CancellationToken.None)).ReturnsAsync(events);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);
        
        var result = await _eventService.GetAllAsync(CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Event1", result.First().Name);
        Assert.Equal("Event2", result.Last().Name);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesEventAndNotifiesParticipants()
    {
        var updateEventDto = new UpdateEventDTO
        {
            Id = Guid.NewGuid(),
            Name = "Updated Event",
            Description = "Updated Description",
            Date = DateTime.UtcNow,
            Location = new Location { City = "Updated City", Street = "Updated Street", Building = "Updated Building" },
            MaxParticipants = 200,
            CategoryId = Guid.NewGuid()
        };

        var eventEntity = new Event
        {
            Id = updateEventDto.Id,
            Name = "Original Event",
            Description = "Original Description",
            Date = DateTime.UtcNow,
            Location = new Location { City = "Original City", Street = "Original Street", Building = "Original Building" },
            MaxParticipants = 100,
            CategoryId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(updateEventDto.Id, CancellationToken.None)).ReturnsAsync(eventEntity);
        _mapperMock.Setup(m => m.Map(updateEventDto, eventEntity));
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);
        _unitOfWorkMock.Setup(uow => uow.Participants.GetByEventIdAsync(updateEventDto.Id, CancellationToken.None))
                       .ReturnsAsync(new List<Participant>());

        await _eventService.UpdateAsync(updateEventDto, CancellationToken.None);

        _unitOfWorkMock.Verify(uow => uow.Events.Update(eventEntity), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
        _emailServiceMock.Verify(es => es.SendEmailAsync(It.IsAny<EventUpdateEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesEvent()
    {
        var eventId = Guid.NewGuid();
        var eventEntity = new Event { Id = eventId, Name = "Event to delete" };

        _unitOfWorkMock.Setup(uow => uow.Events.GetByIdAsync(eventId, CancellationToken.None)).ReturnsAsync(eventEntity);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);
        
        await _eventService.DeleteAsync(eventId, CancellationToken.None);
        
        _unitOfWorkMock.Verify(uow => uow.Events.Delete(eventEntity), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

}
