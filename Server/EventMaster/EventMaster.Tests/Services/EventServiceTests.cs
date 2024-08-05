using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities.Implementations;
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
        // Подготовка
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

        // Действие
        await _eventService.CreateAsync(createEventDto);

        // Проверка
        _unitOfWorkMock.Verify(uow => uow.Events.CreateAsync(eventEntity, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_FiltersByName_ReturnsFilteredEvents()
    {
        // Подготовка
        var filter = new EventFilterDto { Name = "Event1", PageNumber = 1, PageSize = 10 };
        var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Event1" } };
        var eventDtos = new List<EventResponseDTO> { new EventResponseDTO { Id = events.First().Id, Name = "Event1" } };

        _unitOfWorkMock.Setup(uow => uow.Events.GetByNameAsync(filter.Name, filter.PageNumber, filter.PageSize, CancellationToken.None))
                       .ReturnsAsync(events);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);

        // Действие
        var result = await _eventService.GetFilteredEventsAsync(filter, CancellationToken.None);

        // Проверка
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Event1", result.First().Name);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_FiltersByDate_ReturnsFilteredEvents()
    {
        // Подготовка
        var filter = new EventFilterDto { Date = DateTime.UtcNow, PageNumber = 1, PageSize = 10 };
        var events = new List<Event> { new Event { Id = Guid.NewGuid(), Date = filter.Date.Value } };
        var eventDtos = new List<EventResponseDTO> { new EventResponseDTO { Id = events.First().Id, Date = filter.Date.Value } };

        _unitOfWorkMock.Setup(uow => uow.Events.GetEventsQueryableAsync(CancellationToken.None))
            .ReturnsAsync(events.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);

        // Действие
        var result = await _eventService.GetFilteredEventsAsync(filter, CancellationToken.None);

        // Проверка
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(filter.Date.Value, result.First().Date);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_FiltersByLocation_ReturnsFilteredEvents()
    {
        // Подготовка
        var filter = new EventFilterDto
        {
            Location = new Location { City = "City", Street = "Street", Building = "Building" },
            PageNumber = 1,
            PageSize = 10
        };
        var events = new List<Event> { new Event { Id = Guid.NewGuid(), Location = filter.Location } };
        var eventDtos = new List<EventResponseDTO> { new EventResponseDTO { Id = events.First().Id, Location = filter.Location } };

        _unitOfWorkMock.Setup(uow => uow.Events.GetEventsQueryableAsync(CancellationToken.None))
            .ReturnsAsync(events.Where(e => !e.IsDeleted).AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);

        // Действие
        var result = await _eventService.GetFilteredEventsAsync(filter, CancellationToken.None);

        // Проверка
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(filter.Location, result.First().Location);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_FiltersByCategory_ReturnsFilteredEvents()
    {
        // Подготовка
        var filter = new EventFilterDto { CategoryId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), CategoryId = filter.CategoryId.Value }
        }.AsQueryable(); // Убедитесь, что это IQueryable
        var eventDtos = new List<EventResponseDTO>
        {
            new EventResponseDTO { Id = events.First().Id, CategoryId = filter.CategoryId.Value }
        };

        // Настройка мока
        _unitOfWorkMock.Setup(uow => uow.Events.GetEventsQueryableAsync(CancellationToken.None))
            .ReturnsAsync(events); // Возвращаем IQueryable

        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events))
            .Returns(eventDtos);

        // Действие
        var result = await _eventService.GetFilteredEventsAsync(filter, CancellationToken.None);

        // Проверка
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(filter.CategoryId.Value, result.First().CategoryId);
    }


    [Fact]
    public async Task GetAllAsync_ReturnsAllEvents()
    {
        // Подготовка
        var events = new List<Event> { new Event { Id = Guid.NewGuid() } };
        var eventDtos = new List<EventResponseDTO> { new EventResponseDTO { Id = events.First().Id } };

        _unitOfWorkMock.Setup(uow => uow.Events.GetAllAsync(CancellationToken.None)).ReturnsAsync(events);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventResponseDTO>>(events)).Returns(eventDtos);

        // Действие
        var result = await _eventService.GetAllAsync(CancellationToken.None);

        // Проверка
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(events.First().Id, result.First().Id);
    }
}
