using AutoMapper;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.Domain.Entities;

namespace EventMaster.BLL.Infrastructure.Mapper;

public class RoleProfile:Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDTO>();
    }
}