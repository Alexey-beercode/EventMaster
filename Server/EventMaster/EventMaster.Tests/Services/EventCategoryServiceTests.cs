using AutoMapper;
using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.BLL.Services.Implementation;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities.Implementations;
using Moq;

public class EventCategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly EventCategoryService _eventCategoryService;

    public EventCategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _eventCategoryService = new EventCategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEventCategories()
    {
        var eventCategories = new List<EventCategory>
        {
            new EventCategory { Id = Guid.NewGuid(), Name = "Category 1" },
            new EventCategory { Id = Guid.NewGuid(), Name = "Category 2" }
        };
        var eventCategoryDTOs = new List<EventCategoryDTO>
        {
            new EventCategoryDTO { Id = eventCategories[0].Id, Name = "Category 1" },
            new EventCategoryDTO { Id = eventCategories[1].Id, Name = "Category 2" }
        };

        _unitOfWorkMock.Setup(uow => uow.EventCategories.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventCategories);
        _mapperMock.Setup(m => m.Map<List<EventCategoryDTO>>(eventCategories))
            .Returns(eventCategoryDTOs);
        
        var result = await _eventCategoryService.GetAllAsync(CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Category 1", result.ElementAt(0).Name);
        Assert.Equal("Category 2", result.ElementAt(1).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEventCategoryById()
    {
        var eventCategory = new EventCategory { Id = Guid.NewGuid(), Name = "Category 1" };
        var eventCategoryDTO = new EventCategoryDTO { Id = eventCategory.Id, Name = "Category 1" };

        _unitOfWorkMock.Setup(uow => uow.EventCategories.GetByIdAsync(eventCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventCategory);
        _mapperMock.Setup(m => m.Map<EventCategoryDTO>(eventCategory))
            .Returns(eventCategoryDTO);
        
        var result = await _eventCategoryService.GetByIdAsync(eventCategory.Id, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(eventCategory.Id, result.Id);
        Assert.Equal("Category 1", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryNotFound_ReturnsNull()
    {
        var eventCategoryId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.EventCategories.GetByIdAsync(eventCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EventCategory)null);
        
        var result = await _eventCategoryService.GetByIdAsync(eventCategoryId, CancellationToken.None);
        
        Assert.Null(result);
    }
}
