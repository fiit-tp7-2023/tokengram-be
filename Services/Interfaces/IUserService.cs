using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> UpdateUser(string userAddress, UserUpdateRequest request);
    }
}
