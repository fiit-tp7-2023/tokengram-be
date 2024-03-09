using AutoMapper;
using Tokengram.Database.Postgres.Entities;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class ChatInvitationProfile : Profile
    {
        public ChatInvitationProfile()
        {
            CreateMap<ChatInvitation, ChatInvitationResponseDTO>();

            CreateMap<ChatInvitation, ReceivedChatInvitationResponseDTO>();
        }
    }
}
