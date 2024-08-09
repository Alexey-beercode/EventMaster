using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

public class RoleRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public RoleRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                IsDeleted = false
            };

            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(role.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(role);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var role1 = new Role { Id = Guid.NewGuid(), Name = "SecondUser", IsDeleted = false };
            var role2 = new Role { Id = Guid.NewGuid(), Name = "User", IsDeleted = false };
            var role3 = new Role { Id = Guid.NewGuid(), Name = "Guest", IsDeleted = true };

            await context.Roles.AddRangeAsync(role1, role2, role3);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(4);
            result.Should().Contain(r => r.Name == "Admin");
            result.Should().Contain(r => r.Name == "User");
            result.Should().NotContain(r => r.Name == "Guest");
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewRole()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Manager",
                IsDeleted = false
            };

            await repository.CreateAsync(role);
            await context.SaveChangesAsync();

            var result = await context.Roles.FirstOrDefaultAsync(r => r.Id == role.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(role);
        }
    }

    [Fact]
    public void Delete_ShouldMarkRoleAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "ToDelete",
                IsDeleted = false
            };

            context.Roles.Add(role);
            context.SaveChanges();

            repository.Delete(role);
            context.SaveChanges();

            var result = context.Roles.FirstOrDefault(r => r.Id == role.Id);

            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetRolesByUserIdAsync_ShouldReturnRolesForUser()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var userId = Guid.NewGuid();
            var role1 = new Role { Id = Guid.NewGuid(), Name = "Admin", IsDeleted = false };
            var role2 = new Role { Id = Guid.NewGuid(), Name = "User", IsDeleted = false };
            var userRole1 = new UserRole { UserId = userId, RoleId = role1.Id, IsDeleted = false };
            var userRole2 = new UserRole { UserId = userId, RoleId = role2.Id, IsDeleted = false };

            await context.Roles.AddRangeAsync(role1, role2);
            await context.UsersRoles.AddRangeAsync(userRole1, userRole2);
            await context.SaveChangesAsync();

            var result = await repository.GetRolesByUserIdAsync(userId);

            result.Should().Contain(r => r.Name == "Admin");
            result.Should().Contain(r => r.Name == "User");
        }
    }

    [Fact]
    public async Task CheckUserHasRoleAsync_ShouldReturnTrue_WhenUserHasRole()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var userRole = new UserRole { UserId = userId, RoleId = roleId, IsDeleted = false };

            await context.UsersRoles.AddAsync(userRole);
            await context.SaveChangesAsync();

            var result = await repository.CheckUserHasRoleAsync(roleId);

            result.Should().BeTrue();
        }
    }

    [Fact]
    public async Task CheckUserHasRoleAsync_ShouldReturnFalse_WhenUserDoesNotHaveRole()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var result = await repository.CheckUserHasRoleAsync(Guid.NewGuid());

            result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task SetRoleToUserAsync_ShouldAddRoleToUser()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
        
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            
            var user = new User 
            { 
                Id = userId,
                Login = "testuser",
                PasswordHash = "hashedpassword",
                RefreshToken = "somerandomrefreshtoken"
            };
            
            var role = new Role 
            { 
                Id = roleId,
                Name = "Admin",
                IsDeleted = false
            };
            
            context.Users.Add(user);
            context.Roles.Add(role);
            await context.SaveChangesAsync();
            
            await repository.SetRoleToUserAsync(userId, roleId);
            await context.SaveChangesAsync();
            
            var result = await context.UsersRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.RoleId.Should().Be(roleId);
        }
    }


    [Fact]
    public async Task SetRoleToUserAsync_ShouldReturnFalse_WhenRoleOrUserNotFound()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var result=await repository.SetRoleToUserAsync(userId, roleId);
               

            result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task RemoveRoleFromUserAsync_ShouldMarkUserRoleAsDeleted()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userRole = new UserRole { UserId = userId, RoleId = roleId, IsDeleted = false };

            await context.UsersRoles.AddAsync(userRole);
            await context.SaveChangesAsync();

            await repository.RemoveRoleFromUserAsync(userId, roleId);

            var result = await context.UsersRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task RemoveRoleFromUserAsync_ShouldReturnFalse_WhenUserRoleNotFound()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var repository = new RoleRepository(context);
            
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            var result = await repository.RemoveRoleFromUserAsync(userId, roleId);
                

            result.Should().BeFalse();
        }
    }
}
