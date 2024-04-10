using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class PostLikeProfile : Profile
    {
        public PostLikeProfile()
        {
            CreateMap<PostLike, PostLikeResponseDTO>();
        }
    }
}
