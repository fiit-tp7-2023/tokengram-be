using AutoMapper;
using Tokengram.Database.Postgres.Entities;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class ChatMessageProfile : Profile
    {
        public ChatMessageProfile()
        {
            CreateMap<ChatMessage, ChatMessageResponseDTO>();
        }
    }
}
