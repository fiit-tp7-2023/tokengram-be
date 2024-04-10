using AutoMapper;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostWithUserContext, PostResponseDTO>()
                .ForMember(dest => dest.NFT, opt => opt.MapFrom(x => x.Post.NFTQueryResult))
                .ForMember(
                    dest => dest.Description,
                    opt =>
                        opt.MapFrom(x => x.Post.PostUserSettings != null ? x.Post.PostUserSettings.Description : null)
                )
                .ForMember(
                    dest => dest.IsVisible,
                    opt => opt.MapFrom(x => x.Post.PostUserSettings != null && x.Post.PostUserSettings.IsVisible)
                );
        }
    }
}
