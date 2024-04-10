using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public class ChatService : IChatService
    {
        private readonly TokengramDbContext _tokengramDbContext;

        public ChatService(TokengramDbContext tokengramDbContext)
        {
            _tokengramDbContext = tokengramDbContext;
        }

        public async Task<User> GetUserChatProfile(string userAddress)
        {
            return await _tokengramDbContext.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Users)
                .Include(x => x.Chats)
                .ThenInclude(x => x.ChatMessages.OrderByDescending(y => y.CreatedAt).Take(1))
                .ThenInclude(x => x.Sender)
                .FirstAsync(x => x.Address == userAddress);
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessages(
            string userAddress,
            Chat chat,
            PaginationRequestDTO request
        )
        {
            bool isChatMember = await _tokengramDbContext.ChatInvitations.AnyAsync(
                x => x.ChatId == chat.Id && x.UserAddress == userAddress && x.JoinedAt != null
            );

            if (!isChatMember)
                throw new ForbiddenException(Constants.ErrorMessages.CHAT_NOT_MEMBER);

            List<ChatMessage> chatMessages = await _tokengramDbContext.ChatMessages
                .Include(x => x.Sender)
                .Where(x => x.ChatId == chat.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return chatMessages;
        }
    }
}
