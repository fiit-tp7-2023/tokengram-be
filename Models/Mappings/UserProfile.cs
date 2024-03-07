using AutoMapper;
using Tokengram.Database.Postgres.Entities;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDTO>();

            CreateMap<User, UserChatProfileResponseDTO>()
                .ForMember(
                    dest => dest.Chats,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.Chats.Where(
                                    chat =>
                                        src.ReceivedChatInvitations.Any(
                                            chatInvitation => chatInvitation.JoinedAt != null
                                        )
                                )
                        )
                )
                .ForMember(
                    dest => dest.ReceivedChatInvitations,
                    opt => opt.MapFrom(src => src.ReceivedChatInvitations.Where(x => x.JoinedAt == null))
                );
        }
    }
}
