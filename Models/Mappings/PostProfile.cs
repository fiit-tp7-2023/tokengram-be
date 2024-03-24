using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, OwnedPostWithUserContext>();

            CreateMap<OwnedPostWithUserContext, OwnedPostWithUserContextResponseDTO>();
        }
    }
}
