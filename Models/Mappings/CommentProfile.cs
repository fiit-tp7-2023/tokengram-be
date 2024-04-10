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
            CreateMap<Comment, CommentResponseDTO>();

            CreateMap<CommentWithUserContext, CommentResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Comment.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(x => x.Comment.Content))
                .ForMember(dest => dest.Commenter, opt => opt.MapFrom(x => x.Comment.Commenter))
                .ForMember(dest => dest.PostNFTAddress, opt => opt.MapFrom(x => x.Comment.PostNFTAddress))
                .ForMember(x => x.ParentCommentId, opt => opt.MapFrom(x => x.Comment.ParentCommentId))
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(x => x.Comment.CreatedAt));
        }
    }
}
