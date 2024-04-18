using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Models.CustomEntities;

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

            CreateMap<User, BasicUserResponseDTO>()
                .ForMember(
                    dest => dest.ProfilePicture,
                    opt =>
                        opt.MapFrom(
                            (src, dst, d, context) =>
                                src.ProfilePicturePath != null ? uploadsUrlPath + src.ProfilePicturePath : null
                        )
                );

            CreateMap<UserWithUserContext, UserResponseDTO>()
                .ForMember(
                    dest => dest.ProfilePicture,
                    opt =>
                        opt.MapFrom(
                            (src, dst, d, context) =>
                                src.User.ProfilePicturePath != null
                                    ? uploadsUrlPath + src.User.ProfilePicturePath
                                    : null
                        )
                )
                .ForMember(dest => dest.Address, opt => opt.MapFrom(x => x.User.Address))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(x => x.User.Username));

            CreateMap<User, UserChatProfileResponseDTO>()
                .ForMember(
                    dest => dest.Chats,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.Chats.Where(
                                    chat =>
                                        src.ReceivedChatInvitations.Any(
                                            chatInvitation =>
                                                chatInvitation.ChatId == chat.Id && chatInvitation.JoinedAt != null
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
