using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, BasicCommentResponseDTO>();

            CreateMap<Comment, CommentWithUserContext>();

            CreateMap<CommentWithUserContext, CommentWithUserContextResponseDTO>();
        }
    }
}
