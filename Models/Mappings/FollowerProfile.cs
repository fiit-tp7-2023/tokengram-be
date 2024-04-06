using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.CustomEntities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class FollowerProfile : Profile
    {
        public FollowerProfile()
        {
            CreateMap<UserFollow, FollowerResponseDTO>()
                .ForMember(
                    dest => dest.UserAddress,
                    opt => opt.MapFrom(x => x.FollowedUserAddress)
                 )
                .ForMember(
                    dest => dest.FollowingSince,
                    opt => opt.MapFrom(x => x.CreatedAt)
                );
        }
    }
}
