using AutoMapper;
using EventMaster.BLL.DTOs.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Infrastructure.Mapper;

public class UserProfile:Profile
{
    public UserProfile()
    {
        // Mapping from UserDTO to User
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Mapping from User to UserResponseDTO
        CreateMap<User, UserResponseDTO>();
    }
}