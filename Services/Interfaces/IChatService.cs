using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.HTTP.Requests;

namespace Tokengram.Services.Interfaces
{
    public interface IChatService
    {
        Task<User> GetUserChatProfile(string userAddress);

        Task<IEnumerable<ChatMessage>> GetChatMessages(string userAddress, Chat chat, PaginationRequestDTO request);
    }
}
