using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.BLL.Services.Implementation;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using Moq;

namespace EventMaster.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _roleService = new RoleService(_mapperMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetRolesByUserIdAsync_ValidUser_ReturnsRoleDTOs()
    {
        var userId = Guid.NewGuid();
        var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin" },
            new Role { Id = Guid.NewGuid(), Name = "User" }
        };
        var roleDTOs = new List<RoleDTO>
        {
            new RoleDTO { Id = roles[0].Id, Name = roles[0].Name },
            new RoleDTO { Id = roles[1].Id, Name = roles[1].Name }
        };

        _unitOfWorkMock.Setup(uow => uow.Roles.GetRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        _mapperMock.Setup(m => m.Map<IEnumerable<RoleDTO>>(roles)).Returns(roleDTOs);
        
        var result = await _roleService.GetRolesByUserIdAsync(userId);

        Assert.Equal(roleDTOs, result);
        _unitOfWorkMock.Verify(uow => uow.Roles.GetRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CheckUserHasRoleAsync_RoleExists_ReturnsTrue()
    {
        var roleId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Roles.CheckUserHasRoleAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _roleService.CheckUserHasRoleAsync(roleId);
        
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Roles.CheckUserHasRoleAsync(roleId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task SetRoleToUserAsync_ValidUserRole_SetsRoleAndSavesChanges()
    {
        var userRoleDto = new UserRoleDTO { UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        
        _unitOfWorkMock.Setup(uow => uow.Roles.SetRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId, It.IsAny<CancellationToken>()))
           ;
        
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        await _roleService.SetRoleToUserAsync(userRoleDto);
        
        _unitOfWorkMock.Verify(uow => uow.Roles.SetRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    
    [Fact]
    public async Task RemoveRoleFromUserAsync_ValidUserRole_RemovesRoleAndSavesChanges()
    {
        var userRoleDto = new UserRoleDTO { UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Roles.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId, It.IsAny<CancellationToken>()))
            ;
        
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        await _roleService.RemoveRoleFromUserAsync(userRoleDto);
        
        _unitOfWorkMock.Verify(uow => uow.Roles.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    
    [Fact]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin" },
            new Role { Id = Guid.NewGuid(), Name = "User" }
        };
        var roleDTOs = new List<RoleDTO>
        {
            new RoleDTO { Id = roles[0].Id, Name = roles[0].Name },
            new RoleDTO { Id = roles[1].Id, Name = roles[1].Name }
        };

        _unitOfWorkMock.Setup(uow => uow.Roles.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);
        _mapperMock.Setup(m => m.Map<IEnumerable<RoleDTO>>(roles)).Returns(roleDTOs);

        var result = await _roleService.GetAllAsync();

        Assert.Equal(roleDTOs, result);
        _unitOfWorkMock.Verify(uow => uow.Roles.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }



}