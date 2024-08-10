using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Helpers;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.BLL.UseCases.User;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

public class UserServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IConfiguration _configurationMock;

    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly RevokeTokenUseCase _revokeTokenUseCase;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:Secret", "superSecretKey@3459544885322144587" },
                { "Jwt:Issuer", "https://localhost:44300" },
                { "Jwt:Audience", "https://localhost:44300" },
                { "Jwt:Expire", "30" },
                { "Jwt:RefreshTokenExpirationDays", "30" },
                { "ConnectionStrings:ConnectionString", "Host=localhost;Port=5432;Database=EventMaster;Username=postgres;Password=CHEATS145" },
                { "AllowedHosts", "*" }
            });

        _configurationMock = configurationBuilder.Build();

        _registerUserUseCase = new RegisterUserUseCase(_mapperMock.Object, _unitOfWorkMock.Object, _tokenServiceMock.Object, _configurationMock);
        _loginUserUseCase = new LoginUserUseCase(_mapperMock.Object, _unitOfWorkMock.Object, _tokenServiceMock.Object, _configurationMock);
        _refreshTokenUseCase = new RefreshTokenUseCase(_unitOfWorkMock.Object, _tokenServiceMock.Object);
        _revokeTokenUseCase = new RevokeTokenUseCase(_unitOfWorkMock.Object);
        _getAllUsersUseCase = new GetAllUsersUseCase(_mapperMock.Object, _unitOfWorkMock.Object);

        _userService = new UserService(
            _registerUserUseCase,
            _loginUserUseCase,
            _refreshTokenUseCase,
            _revokeTokenUseCase,
            _getAllUsersUseCase);
    }

    [Fact]
    public async Task RegisterAsync_UserExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new User { Login = userDto.Login });

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(() => _userService.RegisterAsync(userDto));
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthorizationException()
    {
        // Arrange
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<AuthorizationException>(() => _userService.LoginAsync(userDto));
    }

    [Fact]
    public async Task LoginAsync_IncorrectPassword_ThrowsAuthorizationException()
    {
        // Arrange
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        var user = new User { Login = userDto.Login, PasswordHash = PasswordHelper.HashPassword("wrongpassword") };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<AuthorizationException>(() => _userService.LoginAsync(userDto));
    }

    [Fact]
    public async Task LoginAsync_ValidUser_ReturnsTokenDTO()
    {
        // Arrange
        var userDto = new UserDTO { Login = "testuser", Password = "password" };
        var tokenDto = new TokenDTO { RefreshToken = "refreshtoken", AccessToken = "accesstoken" };
        var user = new User { Login = userDto.Login, PasswordHash = PasswordHelper.HashPassword("password") };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByLoginAsync(userDto.Login, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role> { new Role { Name = "User" } });
        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns("refreshtoken");
        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns("accesstoken");

        // Act
        var result = await _userService.LoginAsync(userDto);

        // Assert
        Assert.NotNull(result); // Ensure result is not null
        Assert.Equal(tokenDto.RefreshToken, result.RefreshToken);
        Assert.Equal(tokenDto.AccessToken, result.AccessToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokenDTO()
    {
        // Arrange
        var refreshToken = "validrefreshtoken";
        var user = new User { Id = Guid.NewGuid(), RefreshToken = refreshToken };
        var tokenDto = new TokenDTO { RefreshToken = "newrefreshtoken", AccessToken = "newaccesstoken" };

        _unitOfWorkMock.Setup(uow => uow.Users.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<Role> { new Role { Name = "User" } });
        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
                         .Returns("newaccesstoken");

        // Act
        var result = await _userService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(tokenDto.AccessToken, result.AccessToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ThrowsEntityNotFoundException()
    {
        // Arrange
        var refreshToken = "invalidrefreshtoken";
        _unitOfWorkMock.Setup(uow => uow.Users.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.RefreshTokenAsync(refreshToken));
    }

    [Fact]
    public async Task RevokeAsync_ValidUserId_RevokesToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, RefreshToken = "validrefreshtoken" };
        _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

        // Act
        await _userService.RevokeAsync(userId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Users.Update(user), Times.Once);
        Assert.Equal(string.Empty, user.RefreshToken);
        Assert.Equal(DateTime.MinValue, user.RefreshTokenExpiryTime);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfUsers()
    {
        // Arrange
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

        _unitOfWorkMock.Setup(uow => uow.Users.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDTO>>(users))
                   .Returns(userResponseDtos);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("user1", result.ElementAt(0).Login);
        Assert.Equal("user2", result.ElementAt(1).Login);
    }
}
