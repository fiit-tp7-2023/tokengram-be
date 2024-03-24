using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Services.Interfaces
{
    public interface IProfileService
    {
        Task<User> ChangeProfileInfo(string userAddress, ChangeProfileInfoRequestDTO request);
    }
}
