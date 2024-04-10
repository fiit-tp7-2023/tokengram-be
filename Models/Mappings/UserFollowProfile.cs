using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class UserFollowProfile : Profile
    {
        public UserFollowProfile()
        {
            CreateMap<UserFollow, FollowerResponseDTO>()
                .ForMember(dest => dest.FollowingSince, opt => opt.MapFrom(x => x.CreatedAt));

            CreateMap<UserFollow, FollowingResponseDTO>()
                .ForMember(dest => dest.FollowingSince, opt => opt.MapFrom(x => x.CreatedAt));
        }
    }
}
