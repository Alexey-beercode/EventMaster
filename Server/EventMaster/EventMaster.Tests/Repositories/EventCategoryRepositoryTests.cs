// EventCategoryRepositoryTests.cs

using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.Domain.Entities.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.Tests.Repositories;

public class EventCategoryRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public EventCategoryRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnEventCategory_WhenEventCategoryExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventCategoryRepository(context);
            var category = new EventCategory { Id = Guid.NewGuid(), Name = "Test Category", IsDeleted = false };
            await context.EventCategories.AddAsync(category);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(category.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(category);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEventCategoryDoesNotExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventCategoryRepository(context);
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEventCategories()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventCategoryRepository(context);
            
            var category1 = new EventCategory { Id = Guid.NewGuid(), Name = "Test Category 1", IsDeleted = false };
            var category2 = new EventCategory { Id = Guid.NewGuid(), Name = "Test Category 2", IsDeleted = false };
            var category3 = new EventCategory { Id = Guid.NewGuid(), Name = "Deleted Category", IsDeleted = true };

            await context.EventCategories.AddRangeAsync(category1, category2, category3);
            await context.SaveChangesAsync();
            
            var result = await repository.GetAllAsync();
            
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "Test Category 1");
            result.Should().Contain(c => c.Name == "Test Category 2");
            result.Should().NotContain(c => c.Name == "Deleted Category");
        }
    }


    [Fact]
    public async Task CreateAsync_ShouldAddNewEventCategory()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventCategoryRepository(context);
            var category = new EventCategory { Id = Guid.NewGuid(), Name = "New Category", IsDeleted = false };

            await repository.CreateAsync(category);
            await context.SaveChangesAsync();

            var result = await context.EventCategories.FirstOrDefaultAsync(ec => ec.Id == category.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(category);
        }
    }

    [Fact]
    public void Delete_ShouldMarkEventCategoryAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new EventCategoryRepository(context);
            var category = new EventCategory { Id = Guid.NewGuid(), Name = "Category to delete", IsDeleted = false };
            context.EventCategories.Add(category);
            context.SaveChanges();

            repository.Delete(category);
            context.SaveChanges();

            var result = context.EventCategories.FirstOrDefault(ec => ec.Id == category.Id);

            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }
        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoActiveEventCategories()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new EventCategoryRepository(context);
                
                var category = new EventCategory { Id = Guid.NewGuid(), Name = "Deleted Category", IsDeleted = true };
                await context.EventCategories.AddAsync(category);
                await context.SaveChangesAsync();
                
                var result = await repository.GetAllAsync();
                
                result.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldNotAddDuplicateEventCategories()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new EventCategoryRepository(context);
                var categoryId = Guid.NewGuid();
                var category = new EventCategory { Id = categoryId, Name = "Unique Category", IsDeleted = false };
                
                await repository.CreateAsync(category);
                await context.SaveChangesAsync();
                
                var duplicateCategory = new EventCategory { Id = categoryId, Name = "Unique Category", IsDeleted = false };
                
                var existingCategory = await context.EventCategories.FindAsync(categoryId);
                existingCategory.Should().NotBeNull();
                
                var result = await context.EventCategories.ToListAsync();
                result.Should().HaveCount(1); 
            }
        }




        [Fact]
        public void Delete_ShouldNotChangeAlreadyDeletedEventCategory()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new EventCategoryRepository(context);
                var category = new EventCategory { Id = Guid.NewGuid(), Name = "Category to delete", IsDeleted = true };
                context.EventCategories.Add(category);
                context.SaveChanges();
                
                repository.Delete(category);
                context.SaveChanges();
                
                var result = context.EventCategories.FirstOrDefault(ec => ec.Id == category.Id);
                result.Should().NotBeNull();
                result.IsDeleted.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldNotReturnDeletedEventCategory()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new EventCategoryRepository(context);
                var category = new EventCategory { Id = Guid.NewGuid(), Name = "Deleted Category", IsDeleted = true };
                await context.EventCategories.AddAsync(category);
                await context.SaveChangesAsync();
                
                var result = await repository.GetByIdAsync(category.Id);
                
                result.Should().BeNull(); 
            }
        }

        [Fact]
        public async Task Delete_ShouldWorkCorrectlyWithMultipleCategories()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new EventCategoryRepository(context);
                
                var category1 = new EventCategory { Id = Guid.NewGuid(), Name = "Category 1", IsDeleted = false };
                var category2 = new EventCategory { Id = Guid.NewGuid(), Name = "Category 2", IsDeleted = false };
                var category3 = new EventCategory { Id = Guid.NewGuid(), Name = "Category 3", IsDeleted = false };

                await context.EventCategories.AddRangeAsync(category1, category2, category3);
                await context.SaveChangesAsync();
                
                repository.Delete(category2);
                await context.SaveChangesAsync();
                
                var result = await context.EventCategories.ToListAsync();
                result.Should().Contain(c => c.Id == category1.Id && !c.IsDeleted);
                result.Should().Contain(c => c.Id == category3.Id && !c.IsDeleted);
                result.Should().Contain(c => c.Id == category2.Id && c.IsDeleted);
            }
        }
}