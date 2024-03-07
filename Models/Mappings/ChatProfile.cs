using AutoMapper;
using Tokengram.Database.Postgres.Entities;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chat, BasicChatResponseDTO>()
                .ForMember(
                    dest => dest.Users,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.Users.Where(
                                    user =>
                                        src.ChatInvitations
                                            .Where(chatInvitation => chatInvitation.JoinedAt != null)
                                            .Any(chatInvitation => chatInvitation.UserAddress == user.Address)
                                )
                        )
                );

            CreateMap<Chat, ChatResponseDTO>()
                .ForMember(
                    dest => dest.Users,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.Users.Where(
                                    user =>
                                        src.ChatInvitations
                                            .Where(chatInvitation => chatInvitation.JoinedAt != null)
                                            .Any(chatInvitation => chatInvitation.UserAddress == user.Address)
                                )
                        )
                )
                .ForMember(
                    dest => dest.ChatInvitations,
                    opt =>
                        opt.MapFrom(src => src.ChatInvitations.Where(chatInvitation => chatInvitation.JoinedAt == null))
                )
                .ForMember(
                    dest => dest.LastChatMessage,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.ChatMessages
                                    .OrderByDescending(chatMessage => chatMessage.CreatedAt)
                                    .FirstOrDefault()
                        )
                );
        }
    }
}
