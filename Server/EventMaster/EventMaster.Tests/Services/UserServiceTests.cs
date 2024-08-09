using System.Security.Claims;
using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Helpers;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EventMaster.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _configurationMock = new Mock<IConfiguration>();

        _userService = new UserService(_mapperMock.Object, _unitOfWorkMock.Object, _tokenServiceMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_UserExists_ThrowsAuthorizationException()
    {
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync(new User());
        
        await Assert.ThrowsAsync<AuthorizationException>(() => _userService.RegisterAsync(userDto));
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_ReturnsTokenDTO()
    {
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        var user = new User { Id = Guid.NewGuid(), Login = userDto.Login };
        var role = new Role { Id = Guid.NewGuid(), Name = "Resident" };
        var refreshToken = "refreshtoken";
        var accessToken = "accesstoken";
        var configSectionMock = new Mock<IConfigurationSection>();
    
        _unitOfWorkMock.SetupSequence(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null) 
            .ReturnsAsync(user);     

        _unitOfWorkMock.Setup(uow => uow.Users.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Roles.GetByNameAsync("Resident", It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _unitOfWorkMock.Setup(uow => uow.Roles.SetRoleToUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns(refreshToken);
        _tokenServiceMock.Setup(ts => ts.CreateClaims(It.IsAny<User>(), It.IsAny<List<Role>>())).Returns(new List<Claim>());
        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);

        _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserDTO>())).Returns(user);
    
        configSectionMock.Setup(a => a.Value).Returns("7");
        _configurationMock.Setup(c => c.GetSection("Jwt:RefreshTokenExpirationDays")).Returns(configSectionMock.Object);
    
        var result = await _userService.RegisterAsync(userDto);

        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(accessToken, result.AccessToken);
    }



    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthorizationException()
    {
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
    
        await Assert.ThrowsAsync<AuthorizationException>(() => _userService.LoginAsync(userDto));
    }

    [Fact]
    public async Task LoginAsync_IncorrectPassword_ThrowsAuthorizationException()
    {
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        var user = new User { Id = Guid.NewGuid(), Login = userDto.Login, PasswordHash = PasswordHelper.HashPassword("wrongpassword") };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        
        await Assert.ThrowsAsync<AuthorizationException>(() => _userService.LoginAsync(userDto));
    }


    [Fact]
    public async Task LoginAsync_ValidUser_ReturnsTokenDTO()
    {
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        var hashedPassword = PasswordHelper.HashPassword(userDto.Password);
        var user = new User { Id = Guid.NewGuid(), Login = userDto.Login, PasswordHash = hashedPassword };
        var role = new Role { Id = Guid.NewGuid(), Name = "Resident" };
        var refreshToken = "refreshtoken";
        var accessToken = "accesstoken";

        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Users.Update(It.IsAny<User>()));
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _unitOfWorkMock.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Role> { role });
        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns(refreshToken);
        _tokenServiceMock.Setup(ts => ts.CreateClaims(It.IsAny<User>(), It.IsAny<List<Role>>())).Returns(new List<Claim>());
        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);

        var configurationSectionMock = new Mock<IConfigurationSection>();
        configurationSectionMock.Setup(a => a.Value).Returns("7");

        _configurationMock.Setup(c => c.GetSection("Jwt:RefreshTokenExpirationDays")).Returns(configurationSectionMock.Object);
        
        var result = await _userService.LoginAsync(userDto);
        
        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(accessToken, result.AccessToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidUser_ReturnsTokenDTO()
    {
    
        var refreshToken = "refreshtoken";
        var user = new User { Id = Guid.NewGuid(), Login = "testuser", RefreshToken = refreshToken };
        var role = new Role { Id = Guid.NewGuid(), Name = "Resident" };
        var accessToken = "accesstoken";

        _unitOfWorkMock.Setup(uow => uow.Users.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Role> { role });
        _tokenServiceMock.Setup(ts => ts.CreateClaims(It.IsAny<User>(), It.IsAny<List<Role>>())).Returns(new List<Claim>());
        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(accessToken);
    
        var result = await _userService.RefreshTokenAsync(refreshToken);
    
        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(accessToken, result.AccessToken);
    }

    [Fact]
    public async Task RevokeAsync_UserNotFound_ThrowsEntityNotFoundException()
    {
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
    
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.RevokeAsync(userId));
    }

    [Fact]
    public async Task RevokeAsync_ValidUser_SuccessfullyRevokesToken()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Login = "testuser", RefreshToken = "refreshtoken", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7) };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Users.Update(It.IsAny<User>()));
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    
        await _userService.RevokeAsync(userId);

        _unitOfWorkMock.Verify(uow => uow.Users.Update(It.Is<User>(u => u.RefreshToken == string.Empty && u.RefreshTokenExpiryTime == DateTime.MinValue)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task GetAllAsync_ReturnsListOfUsers()
    {
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Login = "user1" },
            new User { Id = Guid.NewGuid(), Login = "user2" }
        };

        var userResponseDtos = new List<UserResponseDTO>
        {
            new UserResponseDTO { Id = users[0].Id, Login = "user1" },
            new UserResponseDTO { Id = users[1].Id, Login = "user2" }
        };

        _unitOfWorkMock.Setup(uow => uow.Users.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDTO>>(users)).Returns(userResponseDtos);
    
        var result = await _userService.GetAllAsync();

        Assert.Equal(userResponseDtos.Count, result.Count());
        Assert.Equal(userResponseDtos[0].Login, result.ElementAt(0).Login);
        Assert.Equal(userResponseDtos[1].Login, result.ElementAt(1).Login);
    }


}