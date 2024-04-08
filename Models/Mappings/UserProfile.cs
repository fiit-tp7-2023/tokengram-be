using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile(IConfiguration _configuration)
        {
            string uploadsUrlPath = _configuration["PublicUploads:UrlPath"] ?? "/";
            if (!uploadsUrlPath.EndsWith("/"))
            {
                uploadsUrlPath += "/";
            }

            CreateMap<User, UserResponseDTO>()
                .ForMember(
                    dest => dest.ProfilePicture,
                    opt =>
                        opt.MapFrom(
                            (src, dst, d, context) =>
                                src.ProfilePicturePath != null ? uploadsUrlPath + src.ProfilePicturePath : null
                        )
                )
                .ForMember(
                    dest => dest.FollowersCount,
                    opt => opt.MapFrom(src => src.Followers.Count)
                )
                .ForMember(
                    dest => dest.FollowingCount,
                    opt => opt.MapFrom(src => src.Followings.Count)
                );

            CreateMap<User, UserChatProfileResponseDTO>()
                .ForMember(
                    dest => dest.Chats,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.Chats.Where(
                                    chat =>
                                        src.ReceivedChatInvitations.Any(
                                            chatInvitation => chatInvitation.JoinedAt != null
                                        )
                                )
                        )
                )
                .ForMember(
                    dest => dest.ReceivedChatInvitations,
                    opt => opt.MapFrom(src => src.ReceivedChatInvitations.Where(x => x.JoinedAt == null))
                );
        }
    }
}
