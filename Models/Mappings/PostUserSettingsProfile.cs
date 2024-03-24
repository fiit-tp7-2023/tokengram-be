using AutoMapper;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Models.Mappings
{
    public class PostUserSettingsProfile : Profile
    {
        public PostUserSettingsProfile()
        {
            CreateMap<PostUserSettings, BasicPostUserSettingsResponseDTO>();
        }
    }
}
