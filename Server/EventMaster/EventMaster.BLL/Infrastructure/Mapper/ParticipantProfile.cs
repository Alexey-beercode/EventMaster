using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.DTOs.Responses.Participant;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Infrastructure.Mapper
{
    public class ParticipantProfile : Profile
    {
        public ParticipantProfile()
        {
            // Mapping from CreateParticipantDTO to Participant
            CreateMap<CreateParticipantDTO, Participant>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore()) 
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Mapping from Participant to ParticipantDTO
            CreateMap<Participant, ParticipantDTO>()
                .ForMember(dest => dest.Event, opt => opt.Ignore());
        }
    }
}