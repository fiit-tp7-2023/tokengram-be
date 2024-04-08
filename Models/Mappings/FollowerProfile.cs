using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class FollowerProfile : Profile
    {
        public FollowerProfile()
        {
            CreateMap<UserFollow, FollowerResponseDTO>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(x => x.FollowedUser)
                 )
                .ForMember(
                    dest => dest.FollowingSince,
                    opt => opt.MapFrom(x => x.CreatedAt)
                );
        }
    }
}
