using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class CommentLikeProfile : Profile
    {
        public CommentLikeProfile()
        {
            CreateMap<CommentLike, BasicCommentLikeResponseDTO>();

            CreateMap<CommentLike, CommentLikeResponseDTO>();
        }
    }
}
