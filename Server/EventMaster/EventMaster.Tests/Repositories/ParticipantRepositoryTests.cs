
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

public class ParticipantRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public ParticipantRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnParticipant_WhenParticipantExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "john.doe@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            await context.Participants.AddAsync(participant);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(participant.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(participant);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenParticipantDoesNotExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllParticipants()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var participant1 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Smith",
                BirthDate = new DateTime(1985, 5, 15),
                RegistrationDate = DateTime.Now,
                Email = "alice.smith@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };
            var participant2 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Johnson",
                BirthDate = new DateTime(1992, 8, 25),
                RegistrationDate = DateTime.Now,
                Email = "bob.johnson@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };
            var participant3 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Brown",
                BirthDate = new DateTime(1988, 12, 30),
                RegistrationDate = DateTime.Now,
                Email = "charlie.brown@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = true
            };

            await context.Participants.AddRangeAsync(participant1, participant2, participant3);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(p => p.FirstName == "Alice");
            result.Should().Contain(p => p.FirstName == "Bob");
            result.Should().NotContain(p => p.FirstName == "Charlie");
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewParticipant()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "New",
                LastName = "Participant",
                BirthDate = new DateTime(2000, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "new.participant@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            await repository.CreateAsync(participant);
            await context.SaveChangesAsync();

            var result = await context.Participants.FirstOrDefaultAsync(p => p.Id == participant.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(participant);
        }
    }

    [Fact]
    public void Delete_ShouldMarkParticipantAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "ToDelete",
                LastName = "Participant",
                BirthDate = new DateTime(1995, 6, 1),
                RegistrationDate = DateTime.Now,
                Email = "to.delete@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            context.Participants.Add(participant);
            context.SaveChanges();

            repository.Delete(participant);
            context.SaveChanges();

            var result = context.Participants.FirstOrDefault(p => p.Id == participant.Id);

            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Update_ShouldModifyParticipant()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Initial",
                LastName = "Participant",
                BirthDate = new DateTime(1990, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "initial.participant@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            context.Participants.Add(participant);
            await context.SaveChangesAsync();

            participant.FirstName = "Updated";
            repository.Update(participant);
            await context.SaveChangesAsync();

            var result = await context.Participants.FirstOrDefaultAsync(p => p.Id == participant.Id);

            result.Should().NotBeNull();
            result.FirstName.Should().Be("Updated");
        }
    }

    [Fact]
    public async Task GetByEventIdAsync_ShouldReturnParticipantsForEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var eventId = Guid.NewGuid();
            var participant1 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 1",
                LastName = "Test",
                BirthDate = new DateTime(1985, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part1@example.com",
                UserId = Guid.NewGuid(),
                EventId = eventId,
                IsDeleted = false
            };
            var participant2 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 2",
                LastName = "Test",
                BirthDate = new DateTime(1986, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part2@example.com",
                UserId = Guid.NewGuid(),
                EventId = eventId,
                IsDeleted = false
            };
            var participant3 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 3",
                LastName = "Test",
                BirthDate = new DateTime(1987, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part3@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(), // Different event
                IsDeleted = false
            };

            await context.Participants.AddRangeAsync(participant1, participant2, participant3);
            await context.SaveChangesAsync();

            var result = await repository.GetByEventIdAsync(eventId);

            result.Should().Contain(p => p.FirstName == "Participant 1");
            result.Should().Contain(p => p.FirstName == "Participant 2");
            result.Should().NotContain(p => p.FirstName == "Participant 3");
        }
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnParticipantsForUser()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var userId = Guid.NewGuid();
            var participant1 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 1",
                LastName = "Test",
                BirthDate = new DateTime(1985, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part1@example.com",
                UserId = userId,
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };
            var participant2 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 2",
                LastName = "Test",
                BirthDate = new DateTime(1986, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part2@example.com",
                UserId = userId,
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };
            var participant3 = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant 3",
                LastName = "Test",
                BirthDate = new DateTime(1987, 1, 1),
                RegistrationDate = DateTime.Now,
                Email = "part3@example.com",
                UserId = Guid.NewGuid(), // Different user
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            await context.Participants.AddRangeAsync(participant1, participant2, participant3);
            await context.SaveChangesAsync();

            var result = await repository.GetByUserId(userId);

            result.Should().Contain(p => p.FirstName == "Participant 1");
            result.Should().Contain(p => p.FirstName == "Participant 2");
            result.Should().NotContain(p => p.FirstName == "Participant 3");
        }
    }

    [Fact]
    public async Task GetByEventIdAsync_ShouldReturnEmpty_WhenNoParticipantsForEvent()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var eventId = Guid.NewGuid();
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant",
                LastName = "Test",
                BirthDate = new DateTime(1995, 6, 1),
                RegistrationDate = DateTime.Now,
                Email = "participant@example.com",
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(), // Different event
                IsDeleted = false
            };

            await context.Participants.AddAsync(participant);
            await context.SaveChangesAsync();

            var result = await repository.GetByEventIdAsync(eventId);

            result.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnEmpty_WhenNoParticipantsForUser()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new ParticipantRepository(context);
            
            var userId = Guid.NewGuid();
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = "Participant",
                LastName = "Test",
                BirthDate = new DateTime(1995, 6, 1),
                RegistrationDate = DateTime.Now,
                Email = "participant@example.com",
                UserId = Guid.NewGuid(), // Different user
                EventId = Guid.NewGuid(),
                IsDeleted = false
            };

            await context.Participants.AddAsync(participant);
            await context.SaveChangesAsync();

            var result = await repository.GetByUserId(userId);

            result.Should().BeEmpty();
        }
    }
}
