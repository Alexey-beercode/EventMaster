using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.Domain.Entities.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                Id = userId,
                Login = "testuser",
                PasswordHash = "hashedpassword",
                RefreshToken = "somerandomrefreshtoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),  // Set expiry time to a future date
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(userId);

            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Login.Should().Be("testuser");
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var userId = Guid.NewGuid();

            var result = await repository.GetByIdAsync(userId);

            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers_WhenUsersExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user1 = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "user1",
                PasswordHash = "hash1",
                RefreshToken = "token1",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            var user2 = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "user2",
                PasswordHash = "hash2",
                RefreshToken = "token2",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(3);
            result.Should().Contain(u => u.Login == "user1");
            result.Should().Contain(u => u.Login == "user2");
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUserToDatabase()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "newuser",
                PasswordHash = "newhash",
                RefreshToken = "newtoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };

            await repository.CreateAsync(user);
            await context.SaveChangesAsync();

            var result = await context.Users.FirstOrDefaultAsync(u => u.Login == "newuser");

            result.Should().NotBeNull();
            result.Login.Should().Be("newuser");
        }
    }

    [Fact]
    public async Task Delete_ShouldMarkUserAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "deleteuser",
                PasswordHash = "deletehash",
                RefreshToken = "deletetoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            repository.Delete(user);
            await context.SaveChangesAsync();

            var result = await context.Users.FirstOrDefaultAsync(u => u.Login == "deleteuser");

            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Update_ShouldModifyUserDetails()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "updateuser",
                PasswordHash = "updatehash",
                RefreshToken = "updatetoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            user.Login = "updateduser";
            repository.Update(user);
            await context.SaveChangesAsync();

            var result = await context.Users.FirstOrDefaultAsync(u => u.Login == "updateduser");

            result.Should().NotBeNull();
            result.Login.Should().Be("updateduser");
        }
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_ShouldReturnUser_WhenUserExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "refreshtokenuser",
                PasswordHash = "refreshhash",
                RefreshToken = "validtoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await repository.GetByRefreshTokenAsync("validtoken");

            result.Should().NotBeNull();
            result.RefreshToken.Should().Be("validtoken");
        }
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_ShouldReturnNull_WhenTokenExpired()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "expiredtokenuser",
                PasswordHash = "expiredhash",
                RefreshToken = "expiredtoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(-1), // Expired token
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await repository.GetByRefreshTokenAsync("expiredtoken");

            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetByLoginAsync_ShouldReturnUser_WhenUserExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "loginuser",
                PasswordHash = "loginhash",
                RefreshToken = "logintoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await repository.GetByLoginAsync("loginuser");

            result.Should().NotBeNull();
            result.Login.Should().Be("loginuser");
        }
    }

    [Fact]
    public async Task GetUsersByRoleIdAsync_ShouldReturnUsersWithRole()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            var roleId = Guid.NewGuid();
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Login = "roleuser",
                PasswordHash = "rolehash",
                RefreshToken = "roletoken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1),
                IsDeleted = false
            };
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            };

            context.Users.Add(user);
            context.UsersRoles.Add(userRole);
            await context.SaveChangesAsync();

            var result = await repository.GetUsersByRoleIdAsync(roleId);

            result.Should().ContainSingle();
            result.First().Id.Should().Be(user.Id);
        }
    }
}
