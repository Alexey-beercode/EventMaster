using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

public class EventRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public EventRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Description",
                Image = sampleImage, 
                IsDeleted = false
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(eventEntity.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(eventEntity);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            var result = await repository.GetByIdAsync(Guid.NewGuid());
            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEvents()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1", Image = sampleImage, IsDeleted = false };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2", Image = sampleImage, IsDeleted = false };
            var event3 = new Event { Id = Guid.NewGuid(), Name = "Deleted Event", Description = "Description 3", Image = sampleImage, IsDeleted = true };

            await context.Events.AddRangeAsync(event1, event2, event3);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Name == "Event 1");
            result.Should().Contain(e => e.Name == "Event 2");
            result.Should().NotContain(e => e.Name == "Deleted Event");
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "New Event",
                Description = "New Description",
                Image = sampleImage, 
                IsDeleted = false
            };

            await repository.CreateAsync(eventEntity);
            await context.SaveChangesAsync();

            var result = await context.Events.FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(eventEntity);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event to Update",
                Description = "Old Description",
                Image = sampleImage, 
                IsDeleted = false
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            eventEntity.Description = "Updated Description";

            repository.Update(eventEntity);
            await context.SaveChangesAsync();

            var result = await context.Events.FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
            result.Should().NotBeNull();
            result.Description.Should().Be("Updated Description");
        }
    }

    [Fact]
    public async Task Delete_ShouldMarkEventAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event to Delete",
                Description = "Description",
                Image = sampleImage, 
                IsDeleted = false
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            repository.Delete(eventEntity);
            await context.SaveChangesAsync();

            var result = await context.Events.FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Delete_ShouldNotChangeAlreadyDeletedEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Already Deleted Event",
                Description = "Description",
                Image = sampleImage, 
                IsDeleted = true
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            repository.Delete(eventEntity);
            await context.SaveChangesAsync();

            var result = await context.Events.FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldNotReturnDeletedEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Deleted Event",
                Description = "Description",
                Image = sampleImage, 
                IsDeleted = true
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(eventEntity.Id);
            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoActiveEvents()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Deleted Event",
                Description = "Description",
                Image = sampleImage, 
                IsDeleted = true
            };

            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();
            result.Should().BeEmpty();
        }
    }
    

    [Fact]
    public async Task GetAllAsync_ShouldReturnEventsInOrder()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event A", Description = "Description A", Image = sampleImage, IsDeleted = false };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event B", Description = "Description B", Image = sampleImage, IsDeleted = false };

            await context.Events.AddRangeAsync(event1, event2);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            result.Should().ContainInOrder(event1, event2);
        }
    }
        [Fact]
    public async Task GetByNameAsync_ShouldReturnEventsMatchingName()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event One", Description = "Description One", Image = sampleImage, IsDeleted = false };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event Two", Description = "Description Two", Image = sampleImage, IsDeleted = false };
            var event3 = new Event { Id = Guid.NewGuid(), Name = "Special Event", Description = "Description Three", Image = sampleImage, IsDeleted = false };

            await context.Events.AddRangeAsync(event1, event2, event3);
            await context.SaveChangesAsync();

            var result = await repository.GetByNameAsync("Event", 1, 2);

            result.Should().Contain(e => e.Name == "Event One");
            result.Should().Contain(e => e.Name == "Event Two");
            result.Should().NotContain(e => e.Name == "Special Event");
        }
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnPagedResults()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event A", Description = "Description A", Image = sampleImage, IsDeleted = false };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event B", Description = "Description B", Image = sampleImage, IsDeleted = false };
            var event3 = new Event { Id = Guid.NewGuid(), Name = "Event C", Description = "Description C", Image = sampleImage, IsDeleted = false };
            var event4 = new Event { Id = Guid.NewGuid(), Name = "Event D", Description = "Description D", Image = sampleImage, IsDeleted = false };

            await context.Events.AddRangeAsync(event1, event2, event3, event4);
            await context.SaveChangesAsync();

            var result = await repository.GetByNameAsync("Event", 2, 2);

            result.Should().Contain(e => e.Name == "Event C");
            result.Should().Contain(e => e.Name == "Event D");
            result.Should().NotContain(e => e.Name == "Event A");
            result.Should().NotContain(e => e.Name == "Event B");
        }
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event A", Description = "Description A", Image = sampleImage, IsDeleted = false };

            await context.Events.AddAsync(event1);
            await context.SaveChangesAsync();

            var result = await repository.GetByNameAsync("Nonexistent", 1, 10);

            result.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetEventsQueryableAsync_ShouldReturnAllNonDeletedEvents()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1", Image = sampleImage, IsDeleted = false };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2", Image = sampleImage, IsDeleted = false };
            var event3 = new Event { Id = Guid.NewGuid(), Name = "Deleted Event", Description = "Description 3", Image = sampleImage, IsDeleted = true };

            await context.Events.AddRangeAsync(event1, event2, event3);
            await context.SaveChangesAsync();

            var queryableResult = await repository.GetEventsQueryableAsync();
            var result = await queryableResult.ToListAsync();

            result.Should().Contain(e => e.Name == "Event 1");
            result.Should().Contain(e => e.Name == "Event 2");
            result.Should().NotContain(e => e.Name == "Deleted Event");
        }
    }
    
    [Fact]
    public async Task GetEventsQueryableAsync_ShouldReturnEmpty_WhenNoActiveEvents()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventRepository(context);
            
            var sampleImage = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };

            var event1 = new Event { Id = Guid.NewGuid(), Name = "Deleted Event", Description = "Description", Image = sampleImage, IsDeleted = true };

            await context.Events.AddAsync(event1);
            await context.SaveChangesAsync();

            var queryableResult = await repository.GetEventsQueryableAsync();
            var result = await queryableResult.ToListAsync();

            result.Should().BeEmpty();
        }
    }
}
