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
                    opt => opt.MapFrom(
                        (src, dst, d, context) => src.ProfilePicturePath != null ?
                            uploadsUrlPath + src.ProfilePicturePath : null
                    )
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
